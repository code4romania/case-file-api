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
    public class BeneficiaryCountQueryHandler : IRequestHandler<BeneficiariesCountCommand, int>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        public BeneficiaryCountQueryHandler(CaseFileContext context, ILogger<UserCountQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Handle(BeneficiariesCountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting the total beneficiaries count for the ngo with the id {request.UserId}");

            IQueryable<Entities.Beneficiary> beneficiaries = _context.Beneficiaries;

            if (request.UserId > 0)
            {
                beneficiaries = beneficiaries.Where(b => b.UserId == request.UserId);
            }

            return await beneficiaries.CountAsync(cancellationToken);
        }
    }
}
