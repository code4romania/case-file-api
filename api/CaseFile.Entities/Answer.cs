using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseFile.Entities
{
    public partial class Answer
    {
        public int UserId { get; set; }        
        public int BeneficiaryId { get; set; }        
        public int OptionToQuestionId { get; set; }        
        public DateTime LastModified { get; set; }
        [MaxLength(1000)]
        public string Value { get; set; }
        
        public virtual User User { get; set; }
        public virtual Beneficiary Beneficiary { get; set; }
        public virtual OptionToQuestion OptionAnswered { get; set; }
    }
}
