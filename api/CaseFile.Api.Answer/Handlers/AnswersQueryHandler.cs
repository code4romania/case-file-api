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
    //IRequestHandler<AnswersQuery, ApiListResponse<AnswerQueryDTO>>,
    IRequestHandler<FilledInAnswersQuery, List<QuestionDTO<FilledInAnswerDTO>>>
    //IRequestHandler<FormAnswersQuery, PollingStationInfosDTO>
    {
        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;

        public AnswersQueryHandler(CaseFileContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //public async Task<ApiListResponse<AnswerQueryDTO>> Handle(AnswersQuery message, CancellationToken cancellationToken)
        //{
        //    var query = _context.Answers.Where(a => a.OptionAnswered.Flagged == message.Urgent);

        //    // Filter by the organizer flag if specified
        //    if (!message.Organizer)
        //    {
        //        query = query.Where(a => a.User.NgoId == message.IdONG);
        //    }

        //    // Filter by county if specified
        //    if (!string.IsNullOrEmpty(message.County))
        //    {
        //        query = query.Where(a => a.CountyCode == message.County);
        //    }

        //    //// Filter by polling station if specified
        //    //if (message.PollingStationNumber > 0)
        //    //{
        //    //    query = query.Where(a => a.PollingStationNumber == message.PollingStationNumber);
        //    //}

        //    // Filter by polling station if specified
        //    if (message.UserId > 0)
        //    {
        //        query = query.Where(a => a.UserId == message.UserId);
        //    }

        //    var answerQueryInfosQuery = query.GroupBy(a => new { a.CountyCode, a.UserId, ObserverName = a.User.LastName }) //a.IdPollingStation, , a.PollingStationNumbers
        //        .Select(x => new CaseFileContext.AnswerQueryInfo
        //        {
        //            IdObserver = x.Key.IdObserver,
        //            //IdPollingStation = x.Key.IdPollingStation,
        //            //PollingStation = $"{x.Key.CountyCode} {x.Key.PollingStationNumber}",
        //            ObserverName = x.Key.ObserverName,
        //            LastModified = x.Max(a => a.LastModified)
        //        });

        //    var count = await answerQueryInfosQuery.CountAsync(cancellationToken: cancellationToken);

        //    var sectiiCuObservatoriPaginat = await answerQueryInfosQuery
        //        .OrderByDescending(aqi => aqi.LastModified)
        //        .Skip((message.Page - 1) * message.PageSize)
        //        .Take(message.PageSize)
        //        .ToListAsync(cancellationToken: cancellationToken);

        //    return new ApiListResponse<AnswerQueryDTO>
        //    {
        //        Data = sectiiCuObservatoriPaginat.Select(x => _mapper.Map<AnswerQueryDTO>(x)).ToList(),
        //        Page = message.Page,
        //        PageSize = message.PageSize,
        //        TotalItems = count
        //    };
        //}


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

        //public async Task<PollingStationInfosDTO> Handle(FormAnswersQuery message, CancellationToken cancellationToken)
        //{
        //    //var raspunsuriFormular = await _context.PollingStationInfos
        //    //    .FirstOrDefaultAsync(rd => rd.UserId == message.UserId
        //    //    && rd.IdPollingStation == message.BeneficiaryId, cancellationToken: cancellationToken);

        //    //return _mapper.Map<PollingStationInfosDTO>(raspunsuriFormular);
        //    return null;
        //}
    }
}