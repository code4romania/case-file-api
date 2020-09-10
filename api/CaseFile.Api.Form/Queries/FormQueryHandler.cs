using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Core.Services;
using CaseFile.Api.Form.Models;
using CaseFile.Entities;

namespace CaseFile.Api.Form.Queries
{
    public class FormQueryHandler :
        IRequestHandler<FormQuestionQuery, IEnumerable<FormSectionDTO>>,
        IRequestHandler<DeleteFormCommand, bool>
        //IRequestHandler<PublishFormCommand, bool>
    {
        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormQueryHandler(CaseFileContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<FormSectionDTO>> Handle(FormQuestionQuery message, CancellationToken cancellationToken)
        {
            var form = _context.Forms.FirstOrDefault(f => f.FormId == message.FormId);
            if (form == null)
            {
                return null;
            }

            // check acces rights
            if (form.Type == FormType.Private)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == message.UserId);
                if (user.Role != Role.Admin && form.CreatedByUser.NgoId != user.NgoId)
                {
                    return null;
                }
                if (form.Draft && form.CreatedByUserId != user.UserId)
                    return null;
            }

            var cacheKey = $"Formular{form.Code}";

            return await _cacheService.GetOrSaveDataInCacheAsync<IEnumerable<FormSectionDTO>>(cacheKey,
                async () =>
                {
                    var r = await _context.Questions
                        .Include(a => a.FormSection)
                        .Include(a => a.OptionsToQuestions)
                        .ThenInclude(a => a.Option)
                        .Where(a => a.FormSection.Form.FormId == message.FormId)
                        .ToListAsync();

                    var sectiuni = r.Select(a => new { IdSectiune = a.SectionId, CodSectiune = a.FormSection.Code, Descriere = a.FormSection.Description }).Distinct();

                    var result = sectiuni.OrderBy(s => s.CodSectiune).Select(i => new FormSectionDTO
                    {
                        SectionId = i.IdSectiune,
                        Title = i.CodSectiune,
                        Description = i.Descriere,
                        Questions = r.Where(a => a.SectionId == i.IdSectiune)
                                     .OrderBy(intrebare => intrebare.QuestionId)
                                     .Select(a => _mapper.Map<QuestionDTO>(a)).ToList()
                    }).ToList();
                    return result;
                },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                });
        }

        public async Task<bool> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
        {
            // the current / logged in user has the right to delete only the forms templates he /she created
            var form = await _context.Forms.Include(f => f.CreatedByUser).FirstOrDefaultAsync(f => f.FormId == request.FormId);
            if (form == null || _context.UserForms.Any(f => f.FormId == form.FormId) || form.CreatedByUserId != request.UserId) // forms assigned to beneficiaries cannot be deleted
            {
                return false;
            }

            var sections = _context.FormSections.Where(s => s.FormId == form.FormId);
            var sectionsIds = sections.Select(s => s.FormSectionId);
            var questions = _context.Questions.Where(q => sectionsIds.Contains(q.SectionId));
            var questionsIds = questions.Select(q => q.QuestionId);
            var optionsToQuestions = _context.OptionsToQuestions.Where(o => questionsIds.Contains(o.QuestionId));
            var optionsIds = optionsToQuestions.Select(o => o.OptionId);
            var options = _context.Options.Where(o => optionsIds.Contains(o.OptionId));
            
            _context.OptionsToQuestions.RemoveRange(optionsToQuestions);
            _context.Options.RemoveRange(options);
            _context.Questions.RemoveRange(questions);
            _context.FormSections.RemoveRange(sections);
            _context.Forms.Remove(form);

            await _context.SaveChangesAsync();
            return true;
        }

        //public async Task<bool> Handle(PublishFormCommand request, CancellationToken cancellationToken)
        //{
        //    var form = await _context.Forms.Include(f => f.CreatedByUser).FirstOrDefaultAsync(f => f.FormId == request.FormId);
        //    if (form == null || form.CreatedByUserId != request.UserId) // check if the current / logged in user has the right to perform this action
        //    {
        //        return false;
        //    }

        //    if (form.Draft)
        //        form.Draft = false;
        //    if (form.Type == FormType.Private)
        //        form.Type = FormType.Public;

        //    await _context.SaveChangesAsync();
        //    return true;
        //}
    }
}