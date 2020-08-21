using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaseFile.Api.Answer.Models
{
    public class AnswerModelWrapper
    {
        public int FormId { get; set; }
        public DateTime CompletionDate { get; set; }
        public BulkAnswerModel[] Answers { get; set; }
    }
    public class SelectedOptionModel
    {
        public int OptionId { get; set; }
        public string Value { get; set; }
    }
    public class BulkAnswerModel
    {
        [Required]
        public int QuestionId { get; set; }        

        [Required(AllowEmptyStrings = false)]
        public int BeneficiaryId { get; set; }

        public List<SelectedOptionModel> Options { get; set; }
    }
}
