using System.Collections.Generic;

namespace CaseFile.Api.Answer.Models
{
    public class AnswerDTO
    {
        public int QuestionId { get; set; }
        public int BeneficiaryId { get; set; }

        public List<SelectedOptionModel> Options { get; set; }
    }
}
