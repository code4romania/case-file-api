using CaseFile.Api.Business.Models;
using CaseFile.Api.Core;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Queries
{
    public class NgoRequestsListQuery : PagingModel
    {
        public bool Pending { get; set; }
    }
}
