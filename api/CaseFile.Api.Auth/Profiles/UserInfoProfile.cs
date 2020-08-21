using AutoMapper;
using CaseFile.Api.Auth.Models;
using CaseFile.Entities;

namespace CaseFile.Api.Auth.Profiles
{
    public class UserInfoProfile : Profile
    {
        public UserInfoProfile()
        {
            //CreateMap<NgoAdmin, UserInfo>()
            //    .ForMember(u => u.Organizer, opt => opt.MapFrom(a => a.Ngo.Organizer));
        }
    }
}