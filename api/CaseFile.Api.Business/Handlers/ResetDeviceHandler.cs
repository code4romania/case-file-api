using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Business.Commands;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Handlers
{
    public class ResetDeviceHandler : IRequestHandler<ResetDeviceCommand, int>
    {
        private readonly CaseFileContext _CaseFileContext;
        private readonly ILogger _logger;

        public ResetDeviceHandler(CaseFileContext context, ILogger<ResetDeviceHandler> logger)
        {
            _CaseFileContext = context;
            _logger = logger;
        }

        public Task<int> Handle(ResetDeviceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var observerQuery = _CaseFileContext.Users
                    .Where(o => o.Phone == request.PhoneNumber);

                if (!request.Organizer)
                {
                    observerQuery = observerQuery.Where(o => o.NgoId == request.IdNgo);
                }

                var observer = observerQuery.FirstOrDefault();

                if (observer == null)
                {
                    return Task.FromResult(-1);
                }

                //observer.DeviceRegisterDate = null;
                //observer.MobileDeviceId = null;

                _CaseFileContext.Update(observer);
                return _CaseFileContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError("Exception caught during resetting of User device for id " + request.PhoneNumber, exception);
            }

            return Task.FromResult(-1);
        }
    }
}
