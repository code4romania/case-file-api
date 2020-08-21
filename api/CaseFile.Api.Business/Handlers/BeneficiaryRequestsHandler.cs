using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Core.Services;
using CaseFile.Api.Business.Commands;
using CaseFile.Entities;
using System.Collections.Generic;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using CSharpFunctionalExtensions;
using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace CaseFile.Api.Business.Handlers
{
    public class BeneficiaryRequestsHandler :        
        IRequestHandler<NewBeneficiaryCommand, int>,
        IRequestHandler<EditBeneficiaryCommand, int>,
        IRequestHandler<GetBeneficiary, Result<BeneficiaryModel>>,
        IRequestHandler<DeleteBeneficiaryCommand, bool>,
        IRequestHandler<SendBeneficiaryInfoCommand, Result<bool>>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;

        public BeneficiaryRequestsHandler(CaseFileContext context, ILogger<UserRequestsHandler> logger, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<int> Handle(NewBeneficiaryCommand message, CancellationToken token)
        {            
            var beneficiary = new Entities.Beneficiary
            {
                BirthDate = message.BirthDate,
                CityId = message.CityId,
                CivilStatus = message.CivilStatus,
                CountyId = message.CountyId,
                Name = message.Name,
                UserId = message.UserId > 0 ? message.UserId : message.CurrentUserId,
                Gender = message.Gender,
                RegistrationDate = DateTime.Now  // todo: use datetime.utcnow and convert using timezone to needed zone date
            };

            if (message.FormsIds.Any())
            {
                var forms = _context.Forms.Where(f => message.FormsIds.Contains(f.FormId)).ToList();
                if (forms == null || forms.Count() == 0)
                    return -1;

                beneficiary.UserForms = new List<UserForm>();
                foreach(var id in message.FormsIds)
                {
                    beneficiary.UserForms.Add(new UserForm { FormId = id, UserId = message.UserId > 0 ? message.UserId : message.CurrentUserId });
                }
            }

            if (message.IsFamilyOfBeneficiaryId != null && message.IsFamilyOfBeneficiaryId > 0)
            {
                var benFromFamily = _context.Beneficiaries.FirstOrDefault(b => b.BeneficiaryId == (int)message.IsFamilyOfBeneficiaryId);

                if (benFromFamily.FamilyId == null)
                {
                    // create new family and assign both beneficiaries to that family
                    var family = new Family { Name = benFromFamily.Name };
                    benFromFamily.Family = family;
                    beneficiary.Family = family;
                }
                else
                {
                    beneficiary.FamilyId = benFromFamily.FamilyId;
                }
            }
            else if (message.FamilyMembersIds.Any())
            {
                var beneficiars = _context.Beneficiaries.Where(b => message.FamilyMembersIds.Contains(b.BeneficiaryId));

                // check they all belong to the same family
                var familyIds = beneficiars.Where(b => b.FamilyId != null).Select(b => b.FamilyId).Distinct().ToList();
                
                if (familyIds == null || familyIds.Count() == 0)
                {
                    // create new family and assign all beneficiaries to that family
                    var family = new Family { Name = beneficiary.Name };                    
                    beneficiary.Family = family;
                    foreach (var ben in beneficiars)
                    {
                        ben.Family = family;
                    }
                }
                else if (familyIds != null && familyIds.Count() == 1)
                {
                    beneficiary.FamilyId = familyIds[0];
                    foreach (var ben in beneficiars)
                    {
                        if (ben.FamilyId == null)
                            ben.FamilyId = familyIds[0];
                    }
                }
                else
                {
                    return -1;
                }
            }

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();
            return beneficiary.BeneficiaryId;
        }

        public async Task<int> Handle(EditBeneficiaryCommand request, CancellationToken cancellationToken)
        {
            var beneficiary = await _context.Beneficiaries.Include(b => b.UserForms).FirstOrDefaultAsync(b => b.BeneficiaryId == request.BeneficiaryId);
            if (beneficiary == null)
            {
                return -1;
            }
            // TODO: handle assistent assignment update; family members update

            if (beneficiary.BirthDate != request.BirthDate)
                beneficiary.BirthDate = request.BirthDate;
            if (beneficiary.CityId != request.CityId)
                beneficiary.CityId = request.CityId;
            if (beneficiary.CivilStatus != request.CivilStatus)
                beneficiary.CivilStatus = request.CivilStatus;
            if (beneficiary.CountyId != request.CountyId)
                beneficiary.CountyId = request.CountyId;
            if (beneficiary.Name != request.Name)
                beneficiary.Name = request.Name;
            if (beneficiary.Gender != request.Gender)
                beneficiary.Gender = request.Gender;
            if (beneficiary.UserId != request.UserId && request.UserId > 0)
                beneficiary.UserId = request.UserId;

            if (request.NewAllocatedFormsIds != null && request.NewAllocatedFormsIds.Any(id => id > 0))
            {
                var forms = _context.Forms.Where(f => request.NewAllocatedFormsIds.Contains(f.FormId)).ToList();
                if (forms == null || forms.Count() == 0)
                    return -1;

                if (beneficiary.UserForms == null)
                    beneficiary.UserForms = new List<UserForm>();
                foreach (var id in request.NewAllocatedFormsIds)
                {
                    beneficiary.UserForms.Add(new UserForm { FormId = id, UserId = beneficiary.UserId });
                }
            }

            if (request.DealocatedFormsIds != null && request.DealocatedFormsIds.Any(id => id > 0))
            {
                var forms = _context.Forms.Where(f => request.DealocatedFormsIds.Contains(f.FormId)).ToList();
                if (forms == null || forms.Count() == 0)
                    return -1;

                foreach (var id in request.DealocatedFormsIds)
                {
                    beneficiary.UserForms.Remove(_context.UserForms.FirstOrDefault(f => f.BeneficiaryId == beneficiary.BeneficiaryId 
                                                    && f.FormId == id 
                                                    && f.UserId == beneficiary.UserId));
                }
            }

            if (request.FamilyMembersIds.Any())
            {
                var beneficiars = _context.Beneficiaries.Where(b => request.FamilyMembersIds.Contains(b.BeneficiaryId));

                // check they all belong to the same family
                var familyIds = beneficiars.Where(b => b.FamilyId != null).Select(b => b.FamilyId).Distinct().ToList();

                if (familyIds == null || familyIds.Count() == 0)
                {
                    // create new family and assign all beneficiaries to that family
                    var family = new Family { Name = beneficiary.Name };
                    beneficiary.Family = family;
                    foreach (var ben in beneficiars)
                    {
                        ben.Family = family;
                    }
                }
                else if (familyIds != null && familyIds.Count() == 1)
                {
                    beneficiary.FamilyId = familyIds[0];
                    foreach (var ben in beneficiars)
                    {
                        if (ben.FamilyId == null)
                            ben.FamilyId = familyIds[0];
                    }
                }
                else
                {
                    return -1;
                }
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<Result<BeneficiaryModel>> Handle(GetBeneficiary request, CancellationToken cancellationToken)
        {
            try
            {
                var beneficiary = await _context.Beneficiaries
                    .Include(b => b.UserForms).ThenInclude(f => f.Form).ThenInclude(f => f.FormSections).ThenInclude(s => s.Questions)
                    .FirstOrDefaultAsync(x => x.BeneficiaryId == request.BeneficiaryId, cancellationToken);
                if (beneficiary == null)
                {
                    return Result.Failure<BeneficiaryModel>($"Could not find beneficiary with id = {request.BeneficiaryId}");
                }

                var beneficiaryModel = new BeneficiaryModel
                {
                    BeneficiaryId = beneficiary.BeneficiaryId,
                    BirthDate = beneficiary.BirthDate,
                    CityId = beneficiary.CityId != null ? (int)beneficiary.CityId : 0,
                    CivilStatus = beneficiary.CivilStatus,
                    CountyId = beneficiary.CountyId != null ? (int)beneficiary.CountyId : 0,
                    Name = beneficiary.Name,
                    UserId = beneficiary.UserId,
                    Gender = beneficiary.Gender,
                    Forms = beneficiary.UserForms != null && beneficiary.UserForms.Any()
                            ? beneficiary.UserForms.Select(f =>
                            new FormModel
                            {
                                Code = f.Form.Code,
                                Date = f.CompletionDate,
                                Description = f.Form.Description,
                                FormId = f.FormId,
                                TotalQuestionsNo = f.Form.FormSections.Sum(s => s.Questions.Count()),
                                QuestionsAnsweredNo = _context.Answers.Include(a => a.OptionAnswered).ThenInclude(o => o.Question).ThenInclude(q => q.FormSection)
                                                    .Where(a => a.BeneficiaryId == beneficiary.BeneficiaryId
                                                    && a.OptionAnswered.Question.FormSection.FormId == f.FormId).Count(),
                                UserName = _context.Users.FirstOrDefault(u => u.UserId == f.UserId).Name
                            }).ToList() : null,
                    FamilyMembers = beneficiary.FamilyId != null && beneficiary.FamilyId > 0
                                    ? _context.Beneficiaries.Where(b => b.FamilyId != null && b.BeneficiaryId != beneficiary.BeneficiaryId && b.FamilyId == beneficiary.FamilyId).Select(b =>
                                    new FamilyMemberModel { BeneficiaryId = b.BeneficiaryId, Name = b.Name }).ToList() : null
                };

                return Result.Ok(beneficiaryModel);
            }
            catch (System.Exception e)
            {
                //_logger.LogError(e.StackTrace);
                _logger.LogError($"Unable to load beneficiary {request.BeneficiaryId}", e);
                return Result.Failure<BeneficiaryModel>($"Unable to load beneficiary {request.BeneficiaryId}");
                //return Result.Failure<BeneficiaryModel>(e.Message);
            }
        }

        public async Task<bool> Handle(DeleteBeneficiaryCommand request, CancellationToken cancellationToken)
        {
            // check that the logged in user is allowed to delete the beneficiary 
            //-> the user is an admin of the ngo where the beneficiary is registered
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (currentUser.Role != Role.NgoAdmin)
            {
                return false;
            }

            var beneficiary = await _context.Beneficiaries.Include(b => b.User).FirstOrDefaultAsync(b => b.BeneficiaryId == request.BeneficiaryId);
            if (beneficiary == null || currentUser.NgoId != beneficiary.User.NgoId)
            {
                return false;
            }

            _context.Notes.RemoveRange(_context.Notes.Where(n => n.BeneficiaryId == request.BeneficiaryId));
            _context.Answers.RemoveRange(_context.Answers.Where(a => a.BeneficiaryId == request.BeneficiaryId));
            _context.UserForms.RemoveRange(_context.UserForms.Where(f => f.BeneficiaryId == request.BeneficiaryId));

            _context.Beneficiaries.Remove(beneficiary);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Result<bool>> Handle(SendBeneficiaryInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // check that the logged in user is allowed to send the file -> the user is the beneficiary's assistent
                // or a member of the ngo where the beneficiary is registered
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);

                var beneficiary = await _context.Beneficiaries.Include(b => b.User).FirstOrDefaultAsync(b => b.BeneficiaryId == request.BeneficiaryId);
                if (beneficiary == null || (currentUser.NgoId != beneficiary.User.NgoId))
                {
                    return Result.Failure<bool>($"Could not find beneficiary with id = {request.BeneficiaryId}");
                }

                var beneficiaryForms = _context.UserForms.Where(f => f.BeneficiaryId == beneficiary.BeneficiaryId).ToList();

                foreach (var form in beneficiaryForms)
                {
                    var hasQuestionsAnswered = _context.Answers.Include(a => a.OptionAnswered).ThenInclude(o => o.Question).ThenInclude(q => q.FormSection)
                                                    .Any(a => a.BeneficiaryId == beneficiary.BeneficiaryId
                                                    && a.OptionAnswered.Question.FormSection.FormId == form.FormId);
                    if (hasQuestionsAnswered)
                    {
                        // todo: add to pdf
                    }
                }

                // send mail with beneficiary info pdf
                var body = "Regasiti atasat formularele completate pentru beneficiarul " + beneficiary.Name;

                await _emailService.Send(currentUser.Email, "Fisa beneficiar", body);
                                
                // TODO! send message with attachement
                //byte[] byteData = File.ReadAllBytes(@"C:\work\CaseFile\src\api\CaseFile.Api.Observer\Files\Test_beneficiary_info.pdf");
                //message.AddAttachment(
                //            new Attachment
                //            {
                //                Content = Convert.ToBase64String(byteData),
                //                Filename = "FisaBeneficiar.pdf",
                //                Type = "application/pdf",
                //                Disposition = "attachment"
                //            });

                return Result.Ok(true);
            }
            catch (System.Exception e)
            {
                //_logger.LogError(e.StackTrace);
                _logger.LogError($"Unable to send beneficiary info for {request.BeneficiaryId}", e);
                return Result.Failure<bool>($"Unable to send beneficiary info for {request.BeneficiaryId}");
                //return Result.Failure<BeneficiaryModel>(e.Message);
            }
        }

    }
}