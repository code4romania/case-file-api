using CaseFile.Api.Core;

namespace CaseFile.Api.Business.Queries
{
    public class BeneficiariesListQuery : PagingModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int Age { get; set; }
    }
}
