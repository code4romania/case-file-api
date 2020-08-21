using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Business.Commands;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Handlers
{
    public class UserCountQueryHandler : IRequestHandler<UserCountCommand, int>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        public UserCountQueryHandler(CaseFileContext context, ILogger<UserCountQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Handle(UserCountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting the total User count for the ngo with the id {request.NgoId}");

            IQueryable<Entities.User> users = _context.Users;

            if (request.NgoId > 0)
            {
                users = users.Where(u => u.NgoId == request.NgoId);
            }

            return await users.CountAsync(cancellationToken);
        }
    }
}
