using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseFile.Entities
{
    public enum FormType
    {
        Private,
        Public
    }

    public partial class Form
    {        
        public int FormId { get; set; }

        public int CreatedByUserId { get; set; }
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        public int CurrentVersion { get; set; }
        public FormType Type { get; set; }
        public bool Draft { get; set; }
		public int Order { get; set; }
        public DateTime Date { get; set; } // creation or publishing date

        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<FormSection> FormSections { get; set; }
        public virtual ICollection<UserForm> UserForms { get; set; }
    }
}
