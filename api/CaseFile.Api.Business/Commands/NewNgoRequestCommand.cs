using AutoMapper;
using MediatR;
using CaseFile.Api.Business.Models;
using CaseFile.Entities;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Commands
{
    public class NewNgoRequestCommand : IRequest<int>
    {
        public string ContactPerson { get; set; }
        public string NgoName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Phone { get; set; }
    }
    public class NgoRequestProfile : Profile
    {
        public NgoRequestProfile()
        {
            CreateMap<NewNgoRequestModel, NewNgoRequestCommand>()
                .ForMember(dest => dest.ContactPerson, c => c.MapFrom(src => src.ContactPerson))
                .ForMember(dest => dest.NgoName, c => c.MapFrom(src => src.NgoName))
                .ForMember(dest => dest.Email, c => c.MapFrom(src => src.Email))
                .ForMember(dest => dest.Message, c => c.MapFrom(src => src.Message))
                .ForMember(dest => dest.Phone, c => c.MapFrom(src => src.Phone))
               ;

            CreateMap<ApproveNgoRequestModel, ApproveNgoRequestCommand>()
                .ForMember(dest => dest.NgoRequestId, c => c.MapFrom(src => src.NgoRequestId))
               ;

            CreateMap<RejectNgoRequestModel, RejectNgoRequestCommand>()
                .ForMember(dest => dest.NgoRequestId, c => c.MapFrom(src => src.NgoRequestId))
               ;

        }
    }

}
