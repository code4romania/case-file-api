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

namespace CaseFile.Api.Business.Handlers
{
    public class UserRequestsHandler :
        IRequestHandler<NewUserCommand, int>,
        IRequestHandler<EditUserCommand, int>,
        IRequestHandler<DeleteUserCommand, bool>,
        IRequestHandler<GetUser, Result<UserModel>>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        private IHashService _hashService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserRequestsHandler(CaseFileContext context, ILogger<UserRequestsHandler> logger, IHashService hashService, IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _hashService = hashService;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<int> Handle(NewUserCommand message, CancellationToken token)
        {
            try
            {
                // check if the current / logged in user has the right to perform this action
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == message.CurrentUserId);
                if (currentUser.Role != Role.Admin && currentUser.Role != Role.NgoAdmin)
                    return -1;

                if (message.Types == null || message.Types.Count() == 0)
                    return -1;

                var password = AccountHelper.GetRandomString(8);

                var user = new Entities.User
                {
                    NgoId = message.NgoId,
                    Phone = message.Phone,
                    Name = message.Name,
                    Email = message.Email,
                    Role = message.Role,
                    CreatedByUserId = message.CurrentUserId,
                    BirthDate = message.BirthDate,
                    TypeList = string.Join<int>(",", message.Types),
                    CreatedOn = DateTime.UtcNow,
                    Password = _hashService.GetHash(password)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // send email with the generated temporary password to the new created user
                var body = "Contul dumneavoastra pentru Dosarul Digital a fost creat cu succes. Aceasta este parola dumneavoastra temporara: "
                    + password
                    + " . Va rugam sa o schimbati cu o parola aleasa de dumneavoastra la prima folosire a aplicatiei.";

                await _emailService.Send(currentUser.Email, "Creare cont", body);

                return user.UserId;
            } 
            catch (Exception ex)
            {
                _logger.LogError("NewUserCommand: ", ex);
                return -1;
            }
           
        }

        public async Task<int> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.CurrentUserId);
            if (request.CurrentUserId != request.UserId && currentUser.Role != Role.Admin && currentUser.Role != Role.NgoAdmin)
                return -1;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (user == null)
            {
                return -1;
            }
            if (user.Name != request.Name)
                user.Name = request.Name;
            if (user.Email != request.Email)
                user.Email = request.Email;
            if (user.Phone != request.Phone)
                user.Phone = request.Phone;
            if (user.Role != request.Role && (currentUser.Role == Role.Admin || currentUser.Role == Role.NgoAdmin))
                user.Role = request.Role;
            if (user.BirthDate != request.BirthDate)
                user.BirthDate = request.BirthDate;
            if (string.IsNullOrEmpty(user.TypeList) || (!string.IsNullOrEmpty(user.TypeList) && user.TypeList.Split(',').Select(int.Parse).ToList().Except(request.Types).Any()))
                user.TypeList = string.Join<int>(",", request.Types);
            
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // check if the current / logged in user has the right to perform this action
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.CurrentUserId);
            if (currentUser.Role != Role.Admin && currentUser.Role != Role.NgoAdmin)
                return false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (user == null)
            {
                return false;
            }
            //_context.Users.Remove(user);

            user.Deleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Result<UserModel>> Handle(GetUser request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);
                if (user == null)
                {
                    return Result.Failure<UserModel>($"Could not find user with id = {request.UserId}");
                }

                var userInfoModel = _mapper.Map<UserModel>(user);
                userInfoModel.Types = user.TypeList != null && user.TypeList.Length > 0 ? user.TypeList.Split(',').Select(int.Parse).ToList() : null;

                return Result.Ok(userInfoModel);
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Unable to load user {request.UserId}", e);
                return Result.Failure<UserModel>($"Unable to load user {request.UserId}");
            }
        }

    }
}