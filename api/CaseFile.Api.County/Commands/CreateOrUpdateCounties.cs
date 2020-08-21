using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CaseFile.Api.County.Commands
{
    public class CreateOrUpdateCounties: IRequest<Result>
    {
        public IFormFile File { get; }

        public CreateOrUpdateCounties(IFormFile file)
        {
            File = file;
        }
    }
}