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
using System;

namespace CaseFile.Api.Business.Handlers
{
    public class BeneficiariesListQueryHandler : IRequestHandler<BeneficiariesListCommand, ApiListResponse<BeneficiarySummaryModel>>,
        IRequestHandler<BeneficiariesDetailsListCommand, ApiListResponse<BeneficiaryDetailsModel>>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public BeneficiariesListQueryHandler(CaseFileContext context, ILogger<BeneficiariesListQueryHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ApiListResponse<BeneficiarySummaryModel>> Handle(BeneficiariesListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Searching for Beneficiaries with the following filters (UserId, Name, Age, City): {request.UserId}, {request.Name}, {request.Age}, {request.City}");

            var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);

            IQueryable<Entities.Beneficiary> beneficiaries = _context.Beneficiaries
                .Include(b => b.User)
                .Include(b => b.County)
                .Include(b => b.City);

            if (request.UserId > 0)
            {
                if (request.AllFromNgo && user.Role == Role.NgoAdmin)
                    beneficiaries = beneficiaries.Where(b => b.User.NgoId == user.NgoId);
                else
                    beneficiaries = beneficiaries.Where(b => b.UserId == request.UserId);
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                beneficiaries = beneficiaries.Where(b => b.Name.Contains(request.Name));
            }

            if (!string.IsNullOrEmpty(request.City))
            {
                beneficiaries = beneficiaries.Where(b => b.City.Name.Contains(request.City));
            }

            if (request.Age > 0)
            {
                var year = DateTime.UtcNow.Year - request.Age;
                beneficiaries = beneficiaries.Where(b => b.BirthDate.Year == year);
            }

            var count = await beneficiaries.CountAsync(cancellationToken);

            var requestedPageBeneficiaries = GetPagedQuery(beneficiaries.OrderBy(b => b.Name), request.Page, request.PageSize)
                .ToList()
                .Select(_mapper.Map<BeneficiarySummaryModel>);


            return new ApiListResponse<BeneficiarySummaryModel>
            {
                TotalItems = count,
                Data = requestedPageBeneficiaries.ToList(),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<ApiListResponse<BeneficiaryDetailsModel>> Handle(BeneficiariesDetailsListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Searching for Beneficiaries with the following filters (UserId, Name, Age, City): {request.UserId}, {request.Name}, {request.Age}, {request.City}");

            IQueryable<Entities.Beneficiary> beneficiaries = _context.Beneficiaries
                .Include(b => b.County)
                .Include(b => b.City)
                .Include(b => b.UserForms).ThenInclude(f => f.Form).ThenInclude(f => f.FormSections).ThenInclude(s => s.Questions);

            if (request.UserId > 0)
            {
                beneficiaries = beneficiaries.Where(b => b.UserId == request.UserId);
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                beneficiaries = beneficiaries.Where(b => b.Name.Contains(request.Name));
            }

            if (!string.IsNullOrEmpty(request.City))
            {
                beneficiaries = beneficiaries.Where(b => b.City.Name.Contains(request.City));
            }

            if (request.Age > 0)
            {
                var year = DateTime.UtcNow.Year - request.Age;
                beneficiaries = beneficiaries.Where(b => b.BirthDate.Year == year);
            }

            var count = await beneficiaries.CountAsync(cancellationToken);

            var requestedPageBeneficiaries = GetPagedQuery(beneficiaries, request.Page, request.PageSize)
                .ToList()
                .Select(b => new BeneficiaryDetailsModel
                {
                    BeneficiaryId = b.BeneficiaryId,
                    BirthDate = b.BirthDate,
                    CityId = b.CityId != null ? (int)b.CityId : 0,
                    CivilStatus = b.CivilStatus,
                    CountyId = b.CountyId != null ? (int)b.CountyId : 0,
                    Name = b.Name,
                    UserId = b.UserId,
                    Gender = b.Gender,
                    Age = (int)((DateTime.UtcNow - b.BirthDate).TotalDays / 365.2425),
                    County = b.CountyId != null ? b.County.Name : null,
                    City = b.CityId != null ? b.City.Name : null,
                    Forms = b.UserForms != null && b.UserForms.Any()
                            ? b.UserForms.Select(f =>
                            new FormModel
                            {
                                Code = f.Form.Code,
                                Date = f.CompletionDate,
                                Description = f.Form.Description,
                                FormId = f.FormId,
                                TotalQuestionsNo = f.Form.FormSections.Sum(s => s.Questions.Count()),
                                QuestionsAnsweredNo = _context.Answers.Include(a => a.OptionAnswered).ThenInclude(o => o.Question).ThenInclude(q => q.FormSection)
                                                    .Where(a => a.BeneficiaryId == b.BeneficiaryId
                                                    && a.OptionAnswered.Question.FormSection.FormId == f.FormId).Count()
                            }).ToList() : null,
                    FamilyMembers = b.FamilyId != null && b.FamilyId > 0
                                    ? _context.Beneficiaries.Where(ben => ben.FamilyId != null && ben.BeneficiaryId != b.BeneficiaryId && ben.FamilyId == b.FamilyId).Select(ben =>
                                    new FamilyMemberModel { BeneficiaryId = ben.BeneficiaryId, Name = ben.Name }).ToList() : null
                });


            return new ApiListResponse<BeneficiaryDetailsModel>
            {
                TotalItems = count,
                Data = requestedPageBeneficiaries.ToList(),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }


        private static IQueryable<Entities.Beneficiary> GetPagedQuery(IQueryable<Entities.Beneficiary> beneficiaries, int page, int pageSize)
        {
            if (pageSize > 0)
            {
                return beneficiaries
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize);
            }

            return beneficiaries;
        }
    }
}
