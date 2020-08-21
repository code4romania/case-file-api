using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Answer.Commands;
using CaseFile.Api.Answer.Models;
using CaseFile.Entities;

namespace CaseFile.Api.Answer.Handlers
{
    public class AnswerQueryHandler :
        IRequestHandler<BulkAnswers, CompleteazaRaspunsCommand>
    {
        private readonly CaseFileContext _context;

        public AnswerQueryHandler(CaseFileContext context)
        {
            _context = context;
        }

        public async Task<CompleteazaRaspunsCommand> Handle(BulkAnswers message, CancellationToken cancellationToken)
        {
            // se identifica beneficiarii pentru care observatorul a adaugat raspunsuri
            var beneficiarsIds = message.Answers
                .Select(a => a.BeneficiaryId)
                .Distinct()
                .ToList();

            var command = new CompleteazaRaspunsCommand { UserId = message.UserId };


            foreach (var beneficiaryId in beneficiarsIds)
            {
                command.Answers.AddRange(message.Answers
                    .Where(a => a.BeneficiaryId == beneficiaryId) 
                    .Select(a => new AnswerDTO
                    {
                        QuestionId = a.QuestionId,
                        BeneficiaryId = beneficiaryId,
                        Options = a.Options                        
                    }));
            }

            return command;
        }
    }
}
