using AutoMapper;
using MediatR;
using CaseFile.Api.Business.Models;
using CaseFile.Entities;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Commands
{
    public class NewUserCommand : IRequest<int>
    {
        public int NgoId { get; set; }
        public int CurrentUserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Role Role { get; set; }
        public DateTime BirthDate { get; set; }
        public List<int> Types { get; set; }
    }
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<NewUserModel, NewUserCommand>()
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, c => c.MapFrom(src => src.Phone))
                .ForMember(dest => dest.NgoId, c => c.MapFrom(src => src.NgoId))
                .ForMember(dest => dest.Email, c => c.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, c => c.MapFrom(src => src.Role))
                .ForMember(dest => dest.BirthDate, c => c.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Types, c => c.MapFrom(src => src.Types))
               ;

            CreateMap<EditUserModel, EditUserCommand>()
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, c => c.MapFrom(src => src.Phone))
                .ForMember(dest => dest.UserId, c => c.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, c => c.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, c => c.MapFrom(src => src.Role))
                .ForMember(dest => dest.BirthDate, c => c.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Types, c => c.MapFrom(src => src.Types))
               ;

            CreateMap<DeleteUserModel, DeleteUserCommand>()
                .ForMember(dest => dest.UserId, c => c.MapFrom(src => src.UserId));
        }
    }

}
