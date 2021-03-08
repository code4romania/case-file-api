using AutoMapper;
using MediatR;
using CaseFile.Api.Core;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using System.Collections.Generic;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Commands
{
    public class NgoRequestsListCommand : IRequest<ApiListResponse<NgoRequestModel>>
    {
        public int UserId { get; set; }
        public bool Pending { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class NgoRequestsCommandProfile : Profile
    {
        public NgoRequestsCommandProfile()
        {
            CreateMap<NgoRequestsListQuery, NgoRequestsListCommand>()                
                .ForMember(dest => dest.Pending, c => c.MapFrom(src => src.Pending))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;
        }
    }
}
