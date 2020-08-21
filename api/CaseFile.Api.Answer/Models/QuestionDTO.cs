using System.Collections.Generic;

namespace CaseFile.Api.Answer.Models
{
    public class QuestionDTO<T>
        where T : class
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public int IdQuestionType { get; set; }
        public string Code { get; set; }
        public int FormId { get; set; }
        //public bool IsMandatory { get; set; }
        //public int? CharsNo { get; set; }

        public IList<T> Answers { get; set; }
    }
}