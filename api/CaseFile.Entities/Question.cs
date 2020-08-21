using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseFile.Entities
{
    public partial class Question
    {
        //public Question()
        //{
        //    Notes = new HashSet<Note>();
        //    OptionsToQuestions = new HashSet<OptionToQuestion>();
        //}
        
        public int QuestionId { get; set; }

        public string Code { get; set; }
        public int SectionId { get; set; }
        public QuestionType QuestionType { get; set; }
        [Required, MaxLength(200)]
        public string Text { get; set; }
        public string Hint { get; set; }
        public bool IsMandatory { get; set; }
        public int? CharsNo { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<OptionToQuestion> OptionsToQuestions { get; set; }
        public virtual FormSection FormSection { get; set; }
    }
}
