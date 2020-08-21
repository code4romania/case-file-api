using CSharpFunctionalExtensions;
using MediatR;
using CaseFile.Api.County.Models;

namespace CaseFile.Api.County.Queries
{
    public class GetCounty : IRequest<Result<CountyModel>>
    {
        public GetCounty(int countyId)
        {
            CountyId = countyId;
        }

        public int CountyId { get;  }
    }
}
