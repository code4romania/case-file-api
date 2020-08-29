using MediatR;
using CaseFile.Api.Business.Models;
using CSharpFunctionalExtensions;

namespace CaseFile.Api.Business.Queries
{
    public class GetBeneficiary : IRequest<Result<BeneficiaryModel>>
    {
        public GetBeneficiary(int beneficiaryId, int userId)
        {
            BeneficiaryId = beneficiaryId;
            UserId = userId;
        }

        public int BeneficiaryId { get; }
        public int UserId { get; }
    }
}
