using CaseFile.Api.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Queries
{
    public class ReportListQuery : PagingModel
    {
        public string Title { get; set; }
    }
}
