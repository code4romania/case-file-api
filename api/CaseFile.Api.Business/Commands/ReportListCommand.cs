using AutoMapper;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using CaseFile.Api.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Commands
{
    public class ReportListCommand : IRequest<ApiListResponse<ReportInfoModel>>
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class ReportListCommandProfile : Profile
    {
        public ReportListCommandProfile()
        {
            CreateMap<ReportListQuery, ReportListCommand>()
                .ForMember(dest => dest.Title, c => c.MapFrom(src => src.Title))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;
        }
    }
}
