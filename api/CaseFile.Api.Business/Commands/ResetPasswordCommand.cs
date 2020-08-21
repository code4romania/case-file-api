using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public int NgoId { get; set; }
        public int UserId { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}