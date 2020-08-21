using System.Collections.Generic;
using CSharpFunctionalExtensions;
using MediatR;
using CaseFile.Api.County.Models;

namespace CaseFile.Api.County.Queries
{
    public class GetCities : IRequest<Result<List<CityModel>>>
    {
        public GetCities(int countyId)
        {
            CountyId = countyId;
        }

        public int CountyId { get; }
    }
}