using AutoMapper;
using MediatR;
using CaseFile.Api.Core;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using CaseFile.Entities;

namespace CaseFile.Api.Business.Commands
{
    public class BeneficiariesDetailsListCommand : IRequest<ApiListResponse<BeneficiaryDetailsModel>>
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }       
        public string City { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class BeneficiariesDetailsListCommandProfile : Profile
    {
        public BeneficiariesDetailsListCommandProfile()
        {
            CreateMap<BeneficiariesListQuery, BeneficiariesDetailsListCommand>()
                .ForMember(dest => dest.City, c => c.MapFrom(src => src.City))
                .ForMember(dest => dest.Age, c => c.MapFrom(src => src.Age))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;
        }
    }
}
