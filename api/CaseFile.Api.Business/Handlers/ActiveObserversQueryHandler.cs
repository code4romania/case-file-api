using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Handlers
{
    public class ActiveObserversQueryHandler : IRequestHandler<ActiveObserversQuery, List<UserModel>>
    {
        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;

        public ActiveObserversQueryHandler(CaseFileContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Task<List<UserModel>> Handle(ActiveObserversQuery request, CancellationToken cancellationToken)
        {
            //var results = _context.PollingStationInfos
            //    .Include(pi => pi.PollingStation)
            //    .Include(pi => pi.PollingStation.County)
            //    .Include(pi => pi.User)
            //    .Where(i => request.CountyCodes.Contains(i.PollingStation.County.Code))
            //    .Where(i => i.PollingStation.Number >= request.FromPollingStationNumber)
            //    .Where(i => i.PollingStation.Number <= request.ToPollingStationNumber);            

            //if (request.NgoId > 0)
            //{
            //    results = results.Where(i => i.User.NgoId == request.NgoId);
            //}

            //var observers = results
            //        .Select(i => i.User)
            //        .AsEnumerable()
            //        .Select(_mapper.Map<ObserverModel>)
            //        .ToList();

            //return Task.FromResult(observers);
            return null;
        }
    }
}
