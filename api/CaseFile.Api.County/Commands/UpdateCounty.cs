using CSharpFunctionalExtensions;
using MediatR;
using CaseFile.Api.County.Models;

namespace CaseFile.Api.County.Commands
{
    public class UpdateCounty : IRequest<Result>
    {
        public CountyModel County { get; }

        public UpdateCounty(int countyId, UpdateCountyModel county)
        {
            County = new CountyModel
            {
                CountyId = countyId,
                Name = county.Name,
                Code = county.Code,
                //Diaspora = county.Diaspora,
                //Order = county.Order,
                //NumberOfPollingStations = county.NumberOfPollingStations
            };
        }
    }
}