using System.Collections.Generic;

namespace CaseFile.Entities
{
    public partial class City
    {
        public int CityId { get; set; }

        public int CountyId { get; set; }
        public string Name { get; set; }

        public virtual County County { get; set; }
    }
}
