using CSharpFunctionalExtensions;
using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class ApproveNgoRequestCommand : IRequest<bool>
    {
        public int NgoRequestId { get; set; }
        public int UserId { get; set; }
    }

    public class RejectNgoRequestCommand : ApproveNgoRequestCommand
    {
        
    }
}
