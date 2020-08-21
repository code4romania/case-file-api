using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Auth.Models;
using CaseFile.Api.Auth.Queries;
using CaseFile.Api.Core.Services;
using CaseFile.Entities;

namespace CaseFile.Api.Auth.Handlers
{
    public class AdminQueryHandler : IRequestHandler<NgoAdminApplicationUser, UserInfo>
    {
        private readonly CaseFileContext _context;
        private readonly IHashService _hash;
        private readonly IMapper _mapper;

        public AdminQueryHandler(CaseFileContext context, IHashService hash, IMapper mapper)
        {
            _context = context;
            _hash = hash;
            _mapper = mapper;
        }

        public async Task<UserInfo> Handle(NgoAdminApplicationUser message, CancellationToken token)
        {
            var hashValue = _hash.GetHash(message.Password);

            //var userinfo = _context.NgoAdmins
            //    .Include(a => a.Ngo)
            //    .Where(a => a.Password == hashValue &&
            //                         a.Account == message.UserName)
            //    .Select(_mapper.Map<UserInfo>)
            //    .FirstOrDefault();

            // TODO!!!

            var userinfo = _context.Users
               .Include(a => a.Ngo)
               .Where(a => a.Password == hashValue &&
                                    a.Email == message.UserName)
               .Select(_mapper.Map<UserInfo>)
               .FirstOrDefault();

            return await Task.FromResult(userinfo);
        }
    }
}
