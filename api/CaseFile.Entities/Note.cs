using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace CaseFile.Entities
{
    public partial class Note
    {
        public int NoteId { get; set; }
        [MaxLength(1000)]
        public string AttachementPath { get; set; }
        public DateTime LastModified { get; set; }
        public int? QuestionId { get; set; }
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public string Text { get; set; }

        public virtual Question Question { get; set; }
        public virtual User User { get; set; }
        public virtual Beneficiary Beneficiary { get; set; }
    }
}
