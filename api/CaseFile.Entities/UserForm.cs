using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Entities
{
    public partial class UserForm
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public int FormId { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime LastSyncDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSynchronized { get; set; }

        public virtual User User { get; set; }
        public virtual Beneficiary Beneficiary { get; set; }
        public virtual Form Form { get; set; }
    }
}
