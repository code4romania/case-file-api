using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Answer.Models;
using CaseFile.Api.Answer.Queries;
using CaseFile.Api.Core;
using CaseFile.Entities;

namespace CaseFile.Api.Answer.Handlers
{
    public class AnswersQueryHandler :
    IRequestHandler<FilledInAnswersQuery, List<QuestionDTO<FilledInAnswerDTO>>>
    {
        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;

        public AnswersQueryHandler(CaseFileContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<QuestionDTO<FilledInAnswerDTO>>> Handle(FilledInAnswersQuery message, CancellationToken cancellationToken)
        {
            List<Entities.Answer> answers = null;
            if (message.FormId > 0)
            {
                answers = await _context.Answers
                    .Include(r => r.OptionAnswered)
                        .ThenInclude(rd => rd.Question)
                        .ThenInclude(q => q.FormSection)
                    .Include(r => r.OptionAnswered)
                        .ThenInclude(rd => rd.Option)
                    .Where(r => r.UserId == message.UserId && r.BeneficiaryId == message.BeneficiaryId && r.OptionAnswered.Question.FormSection.FormId == message.FormId)
                    .ToListAsync(cancellationToken: cancellationToken);
            }
            else
            {
                answers = await _context.Answers
                    .Include(r => r.OptionAnswered)
                        .ThenInclude(rd => rd.Question)
                        .ThenInclude(q => q.FormSection)
                    .Include(r => r.OptionAnswered)
                        .ThenInclude(rd => rd.Option)
                    .Where(r => r.UserId == message.UserId && r.BeneficiaryId == message.BeneficiaryId)
                    .ToListAsync(cancellationToken: cancellationToken);
            }

            var intrebari = answers
                .Select(r => r.OptionAnswered.Question)
                .ToList();

            return intrebari.Select(i => _mapper.Map<QuestionDTO<FilledInAnswerDTO>>(i)).ToList();
        }

    }
}
