using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseFile.Entities
{
    public partial class Ngo
    {
        public int NgoId { get; set; }
        public int? CreatedByUserId { get; set; }
        [MaxLength(10)]
        public string ShortName { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
