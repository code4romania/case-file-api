using MediatR;
using System.Collections.Generic;
using System.Linq;
using CaseFile.Api.Answer.Models;

namespace CaseFile.Api.Answer.Commands
{
    public class BulkAnswers : IRequest<CompleteazaRaspunsCommand>
    {
        public BulkAnswers(IEnumerable<BulkAnswerModel> answers)
        {
            Answers = answers.ToList();
        }

        public int UserId { get; set; }

        public List<BulkAnswerModel> Answers { get; set; }
    }
}