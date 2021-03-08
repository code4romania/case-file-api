using CaseFile.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Models
{
    public class ReportModel
    {
        public int ReportId { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public ReportType ReportType { get; set; }
        public string Center { get; set; }
        public string Period { get; set; }

        public int ActiveCasesLastDayOfPreviousMonth { get; set; }
        public int NewCasesCurrentMonth { get; set; }
        public int ClosedCasesCurrentMonthFamily { get; set; }
        public int ClosedCasesCurrentMonthAssistent { get; set; }
        public int ClosedCasesCurrentMonthOtherOrg { get; set; }
        public int ActiveCasesLastDayOfCurrentMonth { get; set; }

        public int ActiveCasesLastDayOfPreviousMonthUR { get; set; }
        public int NewCasesCurrentMonthUR { get; set; }
        public int ClosedCasesCurrentMonthFamilyUR { get; set; }
        public int ClosedCasesCurrentMonthAssistentUR { get; set; }
        public int ClosedCasesCurrentMonthOtherOrgUR { get; set; }
        public int ActiveCasesLastDayOfCurrentMonthUR { get; set; }

        public int TotalChildrenNo { get; set; }
        public int ChildrenLessThanOneNo { get; set; }
        public int ChildrenOneToTwoNo { get; set; }
        public int ChildrenThreeToSixNo { get; set; }
        public int ChildrenSevenToNineNo { get; set; }
        public int ChildrenTenToThirteenNo { get; set; }
        public int ChildrenFourteenToSeventeenNo { get; set; }
        public int ChildrenEighteenOrMoreNo { get; set; }
    }

    public class ReportInfoModel
    {
        public int ReportId { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string CreationDateString { get; set; }
    }
}
