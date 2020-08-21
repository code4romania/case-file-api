using AutoMapper;
using MediatR;
using CaseFile.Api.Core;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;

namespace CaseFile.Api.Business.Commands
{
    public class UserListCommand : IRequest<ApiListResponse<UserModel>>
    {
        public int NgoId { get; set; }
        //public string Number { get; set; }
        public string Name { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class UserListCommandProfile : Profile
    {
        public UserListCommandProfile()
        {
            CreateMap<UserListQuery, UserListCommand>()
                //.ForMember(dest => dest.Number, c => c.MapFrom(src => src.Number))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;
        }
    }
}
