using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
