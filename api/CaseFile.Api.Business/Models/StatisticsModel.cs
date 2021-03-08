using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Models
{
    public class StatisticsModel
    {
        public int ActiveCasesCurrentMonth { get; set; }
        public int ActiveCasesPreviousMonth { get; set; }
        public int ActiveCasesChildren { get; set; }
    }
}
