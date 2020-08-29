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
            // check if the current / logged in user has the right to perform this action
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (currentUser.Role != Role.Admin && currentUser.Role != Role.NgoAdmin)
                return false;

            var form = await _context.Forms.Include(f => f.CreatedByUser).FirstOrDefaultAsync(f => f.FormId == request.FormId);
            if (form == null || _context.UserForms.Any(f => f.FormId == form.FormId) || form.CreatedByUser.NgoId != currentUser.NgoId) // forms assigned to beneficiaries cannot be deleted
            {
                return false;
            }

            _context.Forms.Remove(form);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}