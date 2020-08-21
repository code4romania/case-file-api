using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseFile.Entities
{
    public partial class OptionToQuestion
    {
        //public OptionToQuestion()
        //{
        //    Answers = new HashSet<Answer>();
        //}

        public int OptionToQuestionId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public bool Flagged { get; set; }

        public virtual ICollection<Answer> Answers { get; }
        public virtual Question Question { get; set; }
        public virtual Option Option { get; set; }
    }
}
