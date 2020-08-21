using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class UserCountCommand : IRequest<int>
    {
        public int NgoId { get; set; }
    }
}
