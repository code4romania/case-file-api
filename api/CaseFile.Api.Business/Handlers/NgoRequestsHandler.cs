using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Core.Services;
using CaseFile.Api.Business.Commands;
using CaseFile.Entities;
using System;
using CSharpFunctionalExtensions;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using AutoMapper;
using CaseFile.Api.Business.Utils;
using System.Collections.Generic;
using CaseFile.Api.Core;

namespace CaseFile.Api.Business.Handlers
{
    public class NgoRequestsHandler :
        IRequestHandler<NgoRequestsListCommand, ApiListResponse<NgoRequestModel>>,
        IRequestHandler<NewNgoRequestCommand, int>,
        IRequestHandler<ApproveNgoRequestCommand, bool>,
        IRequestHandler<RejectNgoRequestCommand, bool>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        private IHashService _hashService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public NgoRequestsHandler(CaseFileContext context, ILogger<NgoRequestsHandler> logger, IMapper mapper, IHashService hashService, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _hashService = hashService;
            _emailService = emailService;
        }

        public async Task<ApiListResponse<NgoRequestModel>> Handle(NgoRequestsListCommand request, CancellationToken cancellationToken)
        {
            // check if the current / logged in user has the right to perform this action
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (currentUser.Role != Role.Admin)
                throw new UnauthorizedAccessException();

            IQueryable<Entities.NgoRequest> ngoRequests = null;

            if (request.Pending)
                ngoRequests = _context.NgoRequests.Where(r => r.RequestStatus == RequestStatus.Pending);
            else
                ngoRequests = _context.NgoRequests.Where(r => r.RequestStatus != RequestStatus.Pending);

            var count = await ngoRequests.CountAsync(cancellationToken);

            var requestedPageRequests = GetPagedQuery(ngoRequests.OrderByDescending(u => u.RequestDate), request.Page, request.PageSize)
                .ToList()
                .Select(_mapper.Map<NgoRequestModel>);


            return new ApiListResponse<NgoRequestModel>
            {
                TotalItems = count,
                Data = requestedPageRequests.ToList(),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<int> Handle(NewNgoRequestCommand message, CancellationToken token)
        {
            try
            {
                var ngoRequest = new Entities.NgoRequest
                {
                    ContactPerson = message.ContactPerson,                    
                    NgoName = message.NgoName,
                    Email = message.Email,
                    Message = message.Message,
                    RequestDate = DateTime.Now,
                    RequestStatus = RequestStatus.Pending,
                    Phone = message.Phone
                };
                _context.NgoRequests.Add(ngoRequest);
                await _context.SaveChangesAsync();                

                return ngoRequest.NgoRequestId;
            } 
            catch (Exception ex)
            {
                _logger.LogError("NewNgoRequestCommand: ", ex);
                return -1;
            }
           
        }

        public async Task<bool> Handle(ApproveNgoRequestCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (currentUser.Role != Role.Admin)
                return false;

            var ngoRequest = await _context.NgoRequests.FirstOrDefaultAsync(r => r.NgoRequestId == request.NgoRequestId);
            if (ngoRequest == null)
            {
                return false;
            }

            ngoRequest.RequestStatus = RequestStatus.Approved;
            ngoRequest.StatusUpdateDate = DateTime.Now;

            // create ngo
            var ngo = new Entities.Ngo
            {
                Name = ngoRequest.NgoName,
                ShortName = ngoRequest.NgoName,
                IsActive = true,
                CreatedByUserId = request.UserId
            };
            _context.Ngos.Add(ngo);

            // create ngo admin account
            var password = AccountHelper.GetRandomString(8);

            var user = new Entities.User
            {
                Ngo = ngo,
                Phone = ngoRequest.Phone,
                Name = ngoRequest.ContactPerson,
                Email = ngoRequest.Email,
                Role = Role.NgoAdmin,
                CreatedByUserId = request.UserId,
                CreatedOn = DateTime.UtcNow,
                Password = _hashService.GetHash(password),
                BirthDate = DateTime.Now
            };
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            // send email with the generated temporary password to the new created user
            var body = "Contul dumneavoastră pentru Dosarul Digital a fost creat cu succes. Această este parola dumneavoastră temporară: "
                + password
                + " . Va rugăm să o schimbați cu o parolă aleasă de dumneavoastră la prima folosire a aplicației. ";
            //+ "<br> De asemenea, pentru a putea folosi aplicatia, trebuie sa introduceti un numar de telefon valid pentru userul dumneavoastra.";

            await _emailService.Send(ngoRequest.Email, "Creare cont", body);
            //await _emailService.Send("cristina.c.stoica@gmail.com", "Creare cont", body);

            return true;
        }

        public async Task<bool> Handle(RejectNgoRequestCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (currentUser.Role != Role.Admin)
                return false;

            var ngoRequest = await _context.NgoRequests.FirstOrDefaultAsync(r => r.NgoRequestId == request.NgoRequestId);
            if (ngoRequest == null)
            {
                return false;
            }

            ngoRequest.RequestStatus = RequestStatus.Rejected;
            ngoRequest.StatusUpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            //// send email with the generated temporary password to the new created user
            //var body = "Solicitarea dumneavoastra pentru creare de cont pentru Dosarul Digital a fost respinsa. Va rugam va rugam sa ne contactati.";

            //await _emailService.Send(currentUser.Email, "Creare cont", body);

            return true;
        }


        private static IQueryable<Entities.NgoRequest> GetPagedQuery(IQueryable<Entities.NgoRequest> requests, int page, int pageSize)
        {
            if (pageSize > 0)
            {
                return requests
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize);
            }

            return requests;
        }

    }
}