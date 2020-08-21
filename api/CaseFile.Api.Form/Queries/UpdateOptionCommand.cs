using MediatR;
using CaseFile.Api.Form.Models;

namespace CaseFile.Api.Form.Queries
{
    public class UpdateOptionCommand : IRequest<int>
    {
        public OptionDto Option { get; }

        public UpdateOptionCommand(OptionDto option)
        {
            Option = option;
        }
    }
}