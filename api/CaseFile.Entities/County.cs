using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaseFile.Entities
{
    public partial class County
    {
        public int CountyId { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
