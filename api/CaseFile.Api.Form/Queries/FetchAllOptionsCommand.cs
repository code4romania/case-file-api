using MediatR;
using System.Collections.Generic;
using CaseFile.Api.Form.Models;

namespace CaseFile.Api.Form.Queries
{
    public class FetchAllOptionsCommand : IRequest<List<OptionDto>>
    {

    }
}