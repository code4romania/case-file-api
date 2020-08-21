using MediatR;
using CaseFile.Api.Business.Models;
using CSharpFunctionalExtensions;

namespace CaseFile.Api.Business.Queries
{
    public class GetBeneficiary : IRequest<Result<BeneficiaryModel>>
    {
        public GetBeneficiary(int beneficiaryId)
        {
            BeneficiaryId = beneficiaryId;
        }

        public int BeneficiaryId { get; }
    }
}
