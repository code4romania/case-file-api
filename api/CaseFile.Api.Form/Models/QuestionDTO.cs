using System.Collections.Generic;
using CaseFile.Entities;

namespace CaseFile.Api.Form.Models
{
    public class QuestionDTO
    {
        public QuestionDTO()
        {
            OptionsToQuestions = new List<OptionToQuestionDTO>();
        }

        public int QuestionId { get; set; }
        //public string FormCode { get; set; }
        public string Code { get; set; }
        public int SectionId { get; set; }
        public QuestionType QuestionType { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public bool IsMandatory { get; set; }
        public int? CharsNo { get; set; }

        public IEnumerable<OptionToQuestionDTO> OptionsToQuestions { get; set; }
    }
}