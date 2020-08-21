using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Core;
using CaseFile.Api.Business.Commands;
using CaseFile.Api.Business.Models;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Handlers
{
    public class UserListQueryHandler : IRequestHandler<UserListCommand, ApiListResponse<UserModel>>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UserListQueryHandler(CaseFileContext context, ILogger<UserListQueryHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ApiListResponse<UserModel>> Handle(UserListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Searching for Users with the following filters (NgoId, Name): {request.NgoId}, {request.Name}");

            IQueryable<Entities.User> users = _context.Users
                .Include(u => u.Ngo)
                .Include(u => u.Notes)
                .Include(u => u.Beneficiaries)
                .Where(u => u.Deleted == false);

            if (request.NgoId > 0)
            {
                users = users.Where(u => u.NgoId == request.NgoId);
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                users = users.Where(u => u.Name.Contains(request.Name));
            }            

            var count = await users.CountAsync(cancellationToken);

            var requestedPageObservers = GetPagedQuery(users.OrderBy(u => u.Name), request.Page, request.PageSize)
                .ToList()
                .Select(_mapper.Map<UserModel>);


            return new ApiListResponse<UserModel>
            {
                TotalItems = count,
                Data = requestedPageObservers.ToList(),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        private static IQueryable<Entities.User> GetPagedQuery(IQueryable<Entities.User> observers, int page, int pageSize)
        {
            if (pageSize > 0)
            {
                return observers
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize);
            }

            return observers;
        }
    }
}
