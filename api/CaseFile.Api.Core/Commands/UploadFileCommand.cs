using MediatR;
using Microsoft.AspNetCore.Http;

namespace CaseFile.Api.Core.Commands
{
    public class UploadFileCommand : IRequest<string>
    {
        public IFormFile File { get; set; }
        public UploadType UploadType { get; set; }
    }
}