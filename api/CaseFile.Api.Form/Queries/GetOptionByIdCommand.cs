using MediatR;
using CaseFile.Api.Form.Models;

namespace CaseFile.Api.Form.Queries
{
    public class GetOptionByIdCommand : IRequest<OptionDto>
    {
        public int OptionId { get; }

        public GetOptionByIdCommand(int optionId)
        {
            OptionId = optionId;
        }
    }
}