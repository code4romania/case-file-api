using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Core.Services;
using CaseFile.Api.Business.Commands;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Handlers
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly CaseFileContext _CaseFileContext;
        private readonly ILogger _logger;
        private readonly IHashService _hash;

        public ResetPasswordHandler(CaseFileContext CaseFileContext, ILogger<ResetPasswordHandler> logger, IHashService hash)
        {
            _CaseFileContext = CaseFileContext;
            _logger = logger;
            _hash = hash;
        }
        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // one can set only his / her own user password
                var user = _CaseFileContext.Users
                    .FirstOrDefault(u => u.UserId == request.UserId && u.NgoId == request.NgoId);

                if (user == null)
                {
                    return false;
                }

                user.Password = _hash.GetHash(request.NewPassword);

                _CaseFileContext.Update(user);
                await _CaseFileContext.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError("Exception caught during resetting of User password for id " + request.UserId, exception);
            }

            return false;
        }
    }
}