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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Layout.Element;
using CaseFile.Api.Answer.Queries;
using CaseFile.Api.Answer.Models;
using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using iText.Kernel.Colors;

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
        private readonly IMapper _mapper;

        public BeneficiaryRequestsHandler(CaseFileContext context, ILogger<UserRequestsHandler> logger, IEmailService emailService, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<int> Handle(NewBeneficiaryCommand message, CancellationToken token)
        {
            // check if operation is allowed
            if(message.UserId > 0 && message.UserId != message.CurrentUserId)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == message.UserId && u.Deleted == false);
                var currentUser = _context.Users.FirstOrDefault(u => u.UserId == message.CurrentUserId && u.Deleted == false);

                if (user.Role == Role.Assistant || user.NgoId != currentUser.NgoId)
                {
                    throw new UnauthorizedAccessException();
                }
            }

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
            // check that the logged in user is allowed to edit the beneficiary
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            
            var beneficiary = await _context.Beneficiaries.Include(b => b.UserForms).FirstOrDefaultAsync(b => b.BeneficiaryId == request.BeneficiaryId);
            if (beneficiary == null || currentUser.NgoId != beneficiary.User.NgoId || (currentUser.UserId != beneficiary.UserId && currentUser.Role == Role.Assistant))
            {
                return -1;
            }
            
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
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(x => x.BeneficiaryId == request.BeneficiaryId, cancellationToken);
                if (beneficiary == null)
                {
                    return Result.Failure<BeneficiaryModel>($"Could not find beneficiary with id = {request.BeneficiaryId}");
                }

                // check that the logged in user is allowed to view the beneficiary info
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
                if (currentUser.NgoId != beneficiary.User.NgoId)
                {
                    throw new UnauthorizedAccessException();
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
                _logger.LogError($"Unable to load beneficiary {request.BeneficiaryId}", e);
                return Result.Failure<BeneficiaryModel>($"Unable to load beneficiary {request.BeneficiaryId}");
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

                var beneficiary = await _context.Beneficiaries.Include(b => b.User).Include(b => b.County).Include(b => b.City).FirstOrDefaultAsync(b => b.BeneficiaryId == request.BeneficiaryId);
                if (beneficiary == null || (currentUser.NgoId != beneficiary.User.NgoId))
                {
                    return Result.Failure<bool>($"Could not find beneficiary with id = {request.BeneficiaryId}");
                }

                var beneficiaryForms = _context.UserForms.Include(f => f.Form).ThenInclude(f => f.FormSections).ThenInclude(s => s.Questions).Where(f => f.BeneficiaryId == beneficiary.BeneficiaryId).ToList();

                byte[] result;

                using (var memoryStream = new MemoryStream())
                {
                    var pdfWriter = new PdfWriter(memoryStream);
                    var pdfDocument = new PdfDocument(pdfWriter);
                    var document = new Document(pdfDocument, PageSize.LETTER, true);

                    document.Add(new Paragraph("Date beneficiar").SetFontSize(15).SetBold());

                    var detailsParagraph = new Paragraph();

                    detailsParagraph.Add(beneficiary.Name).SetFontSize(12).SetBold();
                    detailsParagraph.Add(Environment.NewLine);
                    detailsParagraph.Add(beneficiary.BirthDate.ToString("dd.MM.yyyy"));
                    detailsParagraph.Add(Environment.NewLine);
                    if (beneficiary.CountyId != null)
                    {                        
                        detailsParagraph.Add(beneficiary.County.Name);
                        detailsParagraph.Add(Environment.NewLine);
                    }
                    if (beneficiary.CityId != null)
                    {
                        detailsParagraph.Add(beneficiary.City.Name);
                        detailsParagraph.Add(Environment.NewLine);
                    }

                    var civilStatus = string.Empty;
                    if (beneficiary.CivilStatus == CivilStatus.NotMarried)
                        civilStatus = "necasatorit / a";
                    else if (beneficiary.CivilStatus == CivilStatus.Married)
                        civilStatus = "casatorit / a";
                    else if (beneficiary.CivilStatus == CivilStatus.Divorced)
                        civilStatus = "divortat / a";
                    else if (beneficiary.CivilStatus == CivilStatus.Widowed)
                        civilStatus = "vaduv / a";
                    else if (beneficiary.CivilStatus == CivilStatus.Partner)
                        civilStatus = "concubinaj";

                    detailsParagraph.Add(civilStatus);
                    detailsParagraph.Add(Environment.NewLine);
                    detailsParagraph.Add(Environment.NewLine);

                    document.Add(detailsParagraph);

                    if (!beneficiaryForms.Any())
                    {
                        document.Add(new Paragraph("Nu exista formulare completate pentru acest beneficiar.").SetFontSize(12).SetBold());
                    }
                    else
                    {
                        foreach (var form in beneficiaryForms)
                        {
                            var hasQuestionsAnswered = _context.Answers.Include(a => a.OptionAnswered).ThenInclude(o => o.Question).ThenInclude(q => q.FormSection)
                                                            .Any(a => a.BeneficiaryId == beneficiary.BeneficiaryId
                                                            && a.OptionAnswered.Question.FormSection.FormId == form.FormId);
                            if (hasQuestionsAnswered)
                            {
                                var answers = await _context.Answers
                                    .Include(r => r.OptionAnswered)
                                        .ThenInclude(rd => rd.Question)
                                        .ThenInclude(q => q.FormSection)
                                    .Include(r => r.OptionAnswered)
                                        .ThenInclude(rd => rd.Option)
                                    .Where(r => r.UserId == currentUser.UserId && r.BeneficiaryId == beneficiary.BeneficiaryId && r.OptionAnswered.Question.FormSection.FormId == form.FormId)
                                    .ToListAsync(cancellationToken: cancellationToken);

                                var intrebari = answers
                                    .Select(r => r.OptionAnswered.Question)
                                    .ToList();

                                var formAnswers = intrebari.Select(i => _mapper.Map<QuestionDTO<FilledInAnswerDTO>>(i)).ToList();

                                document.Add(new Paragraph("Formular: " + form.Form.Description).SetFontSize(15).SetBold());

                                foreach (var section in form.Form.FormSections)
                                {
                                    document.Add(new Paragraph(section.Description).SetFontSize(12).SetBold());

                                    if (section.Questions.Any())
                                    {
                                        foreach (var question in section.Questions)
                                        {
                                            var questionParagraph = new Paragraph();
                                            questionParagraph.Add(question.Text);
                                            questionParagraph.Add(Environment.NewLine);

                                            if (formAnswers.Any(a => a.QuestionId == question.QuestionId))
                                            {
                                                var qAnswers = formAnswers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                                                foreach (var answer in qAnswers.Answers)
                                                {
                                                    if (question.QuestionType == QuestionType.Date || question.QuestionType == QuestionType.Number || question.QuestionType == QuestionType.Text)
                                                    {
                                                        if (question.QuestionType == QuestionType.Date && !string.IsNullOrEmpty(answer.Value))
                                                        {
                                                            DateTime date;
                                                            var isDate = DateTime.TryParse(answer.Value, out date);
                                                            questionParagraph.Add(isDate ? date.ToString("dd.MM.yyyy") : " - ");
                                                        }
                                                        else
                                                        {
                                                            questionParagraph.Add(!string.IsNullOrEmpty(answer.Value) ? answer.Value : " - ");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        questionParagraph.Add(answer.Text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                questionParagraph.Add(" - ");
                                                questionParagraph.Add(Environment.NewLine);
                                            }

                                            document.Add(questionParagraph);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    document.Close();

                    result = memoryStream.ToArray();
                }

                // send mail with beneficiary info pdf
                var body = "Regasiti atasat formularele completate pentru beneficiarul " + beneficiary.Name;

                await _emailService.SendWithAttachement(currentUser.Email, "Fisa beneficiar", body, result);                                                

                return Result.Ok(true);
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Unable to send beneficiary info for {request.BeneficiaryId}", e);
                return Result.Failure<bool>($"Unable to send beneficiary info for {request.BeneficiaryId}");
            }
        }

    }
}