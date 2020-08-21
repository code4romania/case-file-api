using CSharpFunctionalExtensions;
using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class SendBeneficiaryInfoCommand : IRequest<Result<bool>>
    {
        public int BeneficiaryId { get; set; }
        public int UserId { get; set; }
    }
}
