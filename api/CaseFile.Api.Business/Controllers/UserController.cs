using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseFile.Api.Core;
using CaseFile.Api.Core.Commands;
using CaseFile.Api.Core.Options;
using CaseFile.Api.Business.Commands;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;

namespace CaseFile.Api.Business.Controllers
{
    [Authorize]
    [Route("api/v1/user")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly DefaultNgoOptions _defaultNgoOptions;

        private int NgoId => this.GetIdOngOrDefault(_defaultNgoOptions.DefaultNgoId);

        private int UserId => this.GetIdObserver();

        public UserController(IMediator mediator, IMapper mapper, IOptions<DefaultNgoOptions> defaultNgoOptions)
        {
            _mediator = mediator;
            _mapper = mapper;
            _defaultNgoOptions = defaultNgoOptions.Value;
        }

        [HttpGet]
        [Produces(type: typeof(ApiListResponse<UserModel>))]
        public async Task<ApiListResponse<UserModel>> GetUsers(UserListQuery query)
        {
            var command = _mapper.Map<UserListCommand>(query);

            command.NgoId = NgoId;

            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet]
        [Produces(type: typeof(int))]
        [Route("count")]
        public async Task<int> GetTotalUserCount()
        {
            var result = await _mediator.Send(new UserCountCommand { NgoId = NgoId });
            return result;
        }
        
        /// <summary>
        ///  Adds an user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The id of the new user.</returns>
        [HttpPost]
        [Produces(type: typeof(int))]
        public async Task<IActionResult> NewUser([FromBody]NewUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newUserCommand = _mapper.Map<NewUserCommand>(model);
            newUserCommand.CurrentUserId = UserId;
            if (model.NgoId == null || model.NgoId <= 0)
                newUserCommand.NgoId = NgoId;
            else
                newUserCommand.NgoId = (int)model.NgoId;
            var newId = await _mediator.Send(newUserCommand);

            return Ok(newId);
        }

        /// <summary>
        /// Edits User information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Boolean indicating whether or not the user was changed successfully</returns>
        [HttpPut]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> EditUser([FromBody]EditUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<EditUserCommand>(model);
            command.CurrentUserId = UserId;
            var id = await _mediator.Send(command);

            return Ok(id > 0);
        }

        /// <summary>
        /// Deletes an user.
        /// </summary>
        /// <param name="userId">The User id</param>
        /// <returns>Boolean indicating whether or not the user was deleted successfully</returns>
        [HttpDelete]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<DeleteUserCommand>(new DeleteUserModel { UserId = userId });
            command.CurrentUserId = UserId;
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> Reset([FromBody]ResetModel model)
        {
            if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword) 
                || model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(new ResetPasswordCommand
            {
                NgoId = NgoId,
                UserId = this.GetIdObserver(),
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            });

            if (result == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserInfoModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [Route("info")]
        public async Task<IActionResult> GetUserAsync()
        {
            var response = await _mediator.Send(new GetUser(UserId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(response.Error);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var response = await _mediator.Send(new GetUser(userId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(response.Error);
        }
    }
}