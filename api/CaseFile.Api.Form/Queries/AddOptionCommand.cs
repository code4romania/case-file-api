using MediatR;
using CaseFile.Api.Form.Models;

namespace CaseFile.Api.Form.Queries
{
    public class AddOptionCommand : IRequest<OptionDto>
    {
        public OptionDto Option { get; }

        public AddOptionCommand(OptionDto option)
        {
            Option = option;
        }
    }
}