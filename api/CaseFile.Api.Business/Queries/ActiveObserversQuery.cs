using AutoMapper;
using MediatR;
using System.Collections.Generic;
using CaseFile.Api.Business.Models;

namespace CaseFile.Api.Business.Queries
{
    public class ActiveObserversQuery : IRequest<List<UserModel>>
    {
        public string[] CountyCodes { get; set; }
        public int FromPollingStationNumber { get; set; }
        public int ToPollingStationNumber { get; set; }
        public bool CurrentlyCheckedIn { get; set; }
        public int IdNgo { get; set; }
    }

    public class ActiveObserverProfile : Profile
    {
        public ActiveObserverProfile()
        {
            CreateMap<ActiveObserverFilter, ActiveObserversQuery>();
            CreateMap<ActiveObserversQuery, ActiveObserverFilter>();
        }
    }
}