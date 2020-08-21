﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CaseFile.Api.Location.Models;

namespace CaseFile.Api.Location.Services
{
    public interface IPollingStationService
    {
        Task<int> GetPollingStationByCountyCode(int pollingStationNumber, string countyCode);
        Task<int> GetPollingStationByCountyId(int pollingStationNumber, int countyId);
        Task<IEnumerable<CountyPollingStationLimit>> GetPollingStationsAssignmentsForAllCounties(bool? diaspora);
    }
}