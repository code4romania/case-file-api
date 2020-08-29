using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using CaseFile.Api.Auth.Models;
using CaseFile.Api.Auth.Queries;
using CaseFile.Api.Core;
using CaseFile.Api.Core.Models;
using CaseFile.Api.Core.Options;
using CaseFile.Api.Auth.Services;
using CaseFile.Api.Business.Queries;

namespace CaseFile.Api.Auth.Controllers
{
    /// <inheritdoc />
    [Route("api/v1/access")]
    public class Authorization : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly MobileSecurityOptions _mobileSecurityOptions;
        public IAuthy _authy;
        private readonly IAccountService _accountService;

        /// <inheritdoc />
        public Authorization(IOptions<JwtIssuerOptions> jwtOptions, ILogger<Authorization> logger, IMediator mediator, 
            IOptions<MobileSecurityOptions> mobileSecurityOptions, IAuthy authy, IAccountService accountService)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = logger;
            _mediator = mediator;
            _mobileSecurityOptions = mobileSecurityOptions.Value;
            _authy = authy;
            _accountService = accountService;
        }

        [HttpPost("authorize")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token;

            // verific daca userul exista si daca da, il returneaza din baza
            var userInfo = await _mediator.Send(new ApplicationUser
            {
                Email = request.Email,
                Password = request.Password
            });

            ClaimsIdentity identity;
            if (!userInfo.IsAuthenticated)
            {
                identity = await Task.FromResult<ClaimsIdentity>(null);
            }

            // Get the generic claims
            identity = new ClaimsIdentity(await GetGenericIdentity(request.Email, userInfo.NgoId.ToString(), UserType.Assistant.ToString()),
                new[]
                {
                new Claim(ClaimsHelper.UserIdProperty, userInfo.UserId.ToString(), ClaimValueTypes.Boolean)
                });

            //var identity = await GetClaimsIdentity(request);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid username ({request.Email}) or password ({request.Password})");
                return Unauthorized("Invalid credentials");
            }

            token = GetTokenFromIdentity(identity);

            var phone = userInfo.Phone.StartsWith('0') ? userInfo.Phone.Remove(0, 1) : userInfo.Phone;

            string result = await _authy.PhoneVerificationRequestAsync(
                                "+40",
                                phone
                            );

            // Serialize and return the response
            var response = new AuthenticationResponseModel
            {
                access_token = token,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds,
                first_login = userInfo.FirstAuthentication,
                phone_verification_request = result
            };

            return Ok(response);
        }

        [HttpPost("verify")]
        [ProducesResponseType(typeof(TokenVerificationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Verify([FromBody]TokenVerificationModel tokenVerification)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.First(c => c.Type == ClaimsHelper.UserIdProperty).Value;
                var response = await _mediator.Send(new GetUser(int.Parse(userId)));
                if (response.IsSuccess)
                {
                    var phone = response.Value.Phone.StartsWith('0') ? response.Value.Phone.Remove(0, 1) : response.Value.Phone;

                    var validationResult = await _authy.VerifyPhoneTokenAsync(
                        phone,
                        "+40",
                        tokenVerification.Token
                    );

                    return Ok(validationResult);
                }
                else
                {
                    return BadRequest(response.Error);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("resend")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResendPhoneVerificationRequest()
        {            
            var userId = User.Claims.First(c => c.Type == ClaimsHelper.UserIdProperty).Value;
            var response = await _mediator.Send(new GetUser(int.Parse(userId)));
            if (response.IsSuccess)
            {
                var phone = response.Value.Phone.StartsWith('0') ? response.Value.Phone.Remove(0, 1) : response.Value.Phone;

                string result = await _authy.PhoneVerificationRequestAsync(
                                "+40",
                                phone
                            );

                return Ok(result);
            }
            else
            {
                return BadRequest(response.Error);
            }            
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public IActionResult ForgotPassword([FromBody]ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _accountService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Va rugam sa va verificati email-ul pentru instructiunile de resetare a parolei" });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public IActionResult ResetPassword([FromBody]ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _accountService.ResetPassword(model);
            return Ok(new { message = "Parola a fost schimbata cu succes, puteti folosi aplicatia." });
        }

        /// <summary>
        /// Test action to get claims
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost("test")]
        //public async Task<object> Test()
        //{
        //    var claims = User.Claims.Select(c => new
        //    {
        //        c.Type,
        //        c.Value
        //    });

        //    return await Task.FromResult(claims);
        //}
        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private async Task<ClaimsIdentity> GetClaimsIdentity(AuthenticateUserRequest request)
        {            
            // verific daca userul exista si daca nu are asociat un alt device, il returneaza din baza
            var userInfo = await _mediator.Send(new ApplicationUser
            {
                Email = request.Email,
                Password = request.Password
            });

            if (!userInfo.IsAuthenticated)
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            // Get the generic claims
            return new ClaimsIdentity(await GetGenericIdentity(request.Email, userInfo.NgoId.ToString(), UserType.Assistant.ToString()),
                new[]
                {
                new Claim(ClaimsHelper.UserIdProperty, userInfo.UserId.ToString(), ClaimValueTypes.Boolean)
                });
        }

        private string GetTokenFromIdentity(ClaimsIdentity identity)
        {
            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: identity.Claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        private async Task<ClaimsIdentity> GetGenericIdentity(string name, string idNgo, string usertype)
        {
            return new ClaimsIdentity(
                new GenericIdentity(name, ClaimsHelper.GenericIdProvider),
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, name),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                        ClaimValueTypes.Integer64),
                    // Custom
                    new Claim(ClaimsHelper.IdNgo, idNgo),
                    new Claim(ClaimsHelper.UserType, usertype)
                });
        }
    }
}
