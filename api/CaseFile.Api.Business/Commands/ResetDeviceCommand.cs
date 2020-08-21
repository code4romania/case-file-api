using MediatR;

namespace CaseFile.Api.Business.Commands
{
    public class ResetDeviceCommand : IRequest<int>
    {
        public int IdNgo { get; set; }
        public string PhoneNumber { get; set; }

        public bool Organizer { get; set; }
    }
}