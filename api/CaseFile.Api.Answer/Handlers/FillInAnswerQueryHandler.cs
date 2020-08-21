using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Answer.Commands;
using CaseFile.Entities;

namespace CaseFile.Api.Answer.Handlers
{
    public class FillInAnswerQueryHandler : IRequestHandler<CompleteazaRaspunsCommand, int>
    {
        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public FillInAnswerQueryHandler(CaseFileContext context, IMapper mapper, ILogger<FillInAnswerQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CompleteazaRaspunsCommand message, CancellationToken cancellationToken)
        {
            try
            {
                //flat answers
                var lastModified = DateTime.UtcNow;

                var raspunsuriNoi = GetFlatListOfAnswers(message, lastModified);

                // stergerea este pe fiecare sectie
                var beneficiarsIds = message.Answers.Select(a => a.BeneficiaryId).Distinct().ToList();

                using (var tran = await _context.Database.BeginTransactionAsync())
                {
                    // save the CompletionDate when the user completes the beneficiary's form 
                    var form = _context.UserForms.FirstOrDefault(f => f.FormId == message.FormId && f.BeneficiaryId == beneficiarsIds[0] && f.UserId == message.UserId);
                    if (form == null)
                        return await Task.FromResult(-1);

                    if (form.CompletionDate != message.CompletionDate)
                        form.CompletionDate = message.CompletionDate;

                    foreach (var beneficiaryId in beneficiarsIds)
                    {
                        var intrebari = message.Answers.Select(a => a.QuestionId).Distinct().ToList();

                        // delete existing answers for posted questions on this 'sectie'
                        var todelete = _context.Answers
                                .Include(a => a.OptionAnswered)
                                .Where(
                                    a =>
                                        a.UserId == message.UserId
                                        && a.BeneficiaryId == beneficiaryId)
                                .Where(a => intrebari.Contains(a.OptionAnswered.QuestionId))
                                .WhereRaspunsContains(intrebari)
                            ;
                        //.Delete();
                        _context.Answers.RemoveRange(todelete);

                        await _context.SaveChangesAsync();
                    }

                    _context.Answers.AddRange(raspunsuriNoi);                    

                    var result = await _context.SaveChangesAsync();

                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(typeof(CompleteazaRaspunsCommand).GetHashCode(), ex, ex.Message);
                //throw (ex);
            }

            return await Task.FromResult(-1);
        }

        public static List<Entities.Answer> GetFlatListOfAnswers(CompleteazaRaspunsCommand command, DateTime lastModified)
        {
            var list = command.Answers.Select(a => new
            {
                flat = a.Options.Select(o => new Entities.Answer
                {
                    UserId = command.UserId,
                    BeneficiaryId = a.BeneficiaryId,
                    OptionToQuestionId = o.OptionId,
                    Value = o.Value,
                    //CountyCode = a.CountyCode,
                    //PollingStationNumber = a.PollingStationNumber,
                    LastModified = lastModified
                })
            })
                .SelectMany(a => a.flat)
                .GroupBy(k => k.OptionToQuestionId,
                    (g, o) => new Entities.Answer
                    {
                        UserId = command.UserId,
                        BeneficiaryId = o.Last().BeneficiaryId,
                        OptionToQuestionId = g,
                        Value = o.Last().Value,
                        //CountyCode = o.Last().CountyCode,
                        //PollingStationNumber = o.Last().PollingStationNumber,
                        LastModified = lastModified
                    })
                .Distinct()
                .ToList();

            return list;
        }
    }
}
