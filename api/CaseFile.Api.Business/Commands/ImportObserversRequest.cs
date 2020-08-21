using MediatR;
using Microsoft.AspNetCore.Http;

namespace CaseFile.Api.Business.Commands
{
    public class ImportObserversRequest : IRequest<int>
    {
        public int IdOng { get; set; }
        public IFormFile File { get; set; }
    }
}
