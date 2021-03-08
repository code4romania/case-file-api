using AutoMapper;
using CaseFile.Api.Business.Models;
using CaseFile.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Commands
{
    public class NewReportCommand : IRequest<int>
    {
        public int CurrentUserId { get; set; }
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

    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<NewReportModel, NewReportCommand>();
            CreateMap<NewReportCommand, Report>();
            CreateMap<Report, ReportModel>();

            CreateMap<Report, ReportInfoModel>()
                .ForMember(dest => dest.ReportId, c => c.MapFrom(src => src.ReportId))
                .ForMember(dest => dest.Title, c => c.MapFrom(src => src.Title))
                .ForMember(dest => dest.UserName, c => c.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.CreationDateString, c => c.MapFrom(src => src.CreationDate.ToString("dd.MM.yyyy")));
        }
    }
}
