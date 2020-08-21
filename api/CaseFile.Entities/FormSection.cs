using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseFile.Entities
{
    public partial class FormSection
    {        
        public int FormSectionId { get; set; }
        [MaxLength(200)]
        public string Code { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public int FormId { get; set; }
        public virtual Form Form { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        //public FormSection()
        //{
        //    Questions = new HashSet<Question>();
        //}
    }
}
