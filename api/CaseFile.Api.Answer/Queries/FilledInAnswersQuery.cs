using MediatR;
using System.Collections.Generic;
using CaseFile.Api.Answer.Models;

namespace CaseFile.Api.Answer.Queries
{
    public class FilledInAnswersQuery : IRequest<List<QuestionDTO<FilledInAnswerDTO>>>
    {
        public int BeneficiaryId { get; set; }
        public int UserId { get; set; }
        public int? FormId { get; set; }
    }
}