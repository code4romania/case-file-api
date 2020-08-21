using System.Collections.Generic;
using CSharpFunctionalExtensions;
using MediatR;
using CaseFile.Api.County.Models;

namespace CaseFile.Api.County.Queries
{
    public class GetAllCounties : IRequest<Result<List<CountyModel>>>
    {
    }
}