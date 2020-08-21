using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class DeleteBeneficiaryCommand : IRequest<bool>
    {
        public int BeneficiaryId { get; set; }
        public int UserId { get; set; }
    }
}
