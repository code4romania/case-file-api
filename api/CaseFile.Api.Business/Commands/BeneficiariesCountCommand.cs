using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class BeneficiariesCountCommand : IRequest<int>
    {
        public int UserId { get; set; }
    }
}
