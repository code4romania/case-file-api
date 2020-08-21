using AutoMapper;
using CaseFile.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaseFile.Api.Business.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string Ngo { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string Phone { get; set; }
        public Role Role { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? LastSeen { get; set; }
        public int NumberOfNotes { get; set; }
        public int NumberOfBeneficiaries { get; set; }
        public ICollection<int> Types { get; set; }
    }

    public class UserInfoModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
    }

    public class UserModelProfile : Profile
    {
        public UserModelProfile()
        {
            CreateMap<Entities.User, UserModel>()
                .ForMember(dest => dest.Ngo, c => c.MapFrom(src => src.Ngo.Name))
                .ForMember(dest => dest.UserId, c => c.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.PictureUrl, c => c.MapFrom(src => src.PictureUrl))
                .ForMember(dest => dest.Phone, c => c.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Role, c => c.MapFrom(src => src.Role))
                .ForMember(dest => dest.BirthDate, c => c.MapFrom(src => src.BirthDate))
                //.ForMember(dest => dest.Types, c => c.MapFrom(src => src.TypeList))
                .ForMember(dest => dest.NumberOfNotes, c => c.MapFrom(src => src.Notes.Count))
                .ForMember(dest => dest.NumberOfBeneficiaries, c => c.MapFrom(src => src.Beneficiaries.Count));

            CreateMap<Entities.User, UserInfoModel>()
                .ForMember(dest => dest.UserId, c => c.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.PictureUrl, c => c.MapFrom(src => src.PictureUrl));
        }
    }
}
