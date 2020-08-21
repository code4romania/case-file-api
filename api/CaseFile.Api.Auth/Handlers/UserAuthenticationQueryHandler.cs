using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Auth.Models;
using CaseFile.Api.Auth.Queries;
using CaseFile.Api.Core.Options;
using CaseFile.Api.Core.Services;
using CaseFile.Entities;
using Microsoft.Extensions.Logging;
using System;

namespace CaseFile.Api.Auth.Handlers
{
    /// <summary>
    /// Handles the query regarding the authentication of the user - checks the phone number and hashed pin against the database
    /// </summary>
    public class UserAuthenticationQueryHandler : IRequestHandler<ApplicationUser, RegisteredUserModel>
    {
        private readonly CaseFileContext _context;
        private readonly IHashService _hash;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for dependency injection
        /// </summary>
        /// <param name="context">The EntityFramework context</param>
        /// <param name="hash">Implementation of the IHashService to be used to generate the hashes. It can either be `HashService` or `ClearTextService`.</param>
        public UserAuthenticationQueryHandler(CaseFileContext context, IHashService hash, ILogger<UserAuthenticationQueryHandler> logger)
        {
            _context = context;
            _hash = hash;            
            _logger = logger;
        }

        public async Task<RegisteredUserModel> Handle(ApplicationUser message, CancellationToken cancellationToken)
        {
            var hashValue = _hash.GetHash(message.Password);

            // Check for username and hash
            var userQuery = _context.Users.Where(u => u.Password == hashValue && u.Email.Trim() == message.Email.Trim() && u.Deleted == false);

            var userinfo = await userQuery.FirstOrDefaultAsync<User>(cancellationToken: cancellationToken);
            bool firstLogin = false;
            if (userinfo == null)
            {
                return new RegisteredUserModel
                {
                    IsAuthenticated = false
                };
            }
            else
            {
                if (userinfo.LastSignIn == null)
                    firstLogin = true;

                // update user's LastSignIn date
                userinfo.LastSignIn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return new RegisteredUserModel
            {
                UserId = userinfo.UserId,
                NgoId = userinfo.NgoId,
                IsAuthenticated = true,
                FirstAuthentication = firstLogin,
                Phone = userinfo.Phone
            };
        }
    }
}
