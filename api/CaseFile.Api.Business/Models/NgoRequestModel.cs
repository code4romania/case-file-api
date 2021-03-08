using AutoMapper;
using CaseFile.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Models
{
    public class NgoRequestModel
    {
        public int NgoRequestId { get; set; }
        public string ContactPerson { get; set; }
        public string NgoName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string RequestDate { get; set; }
        public string StatusUpdateDate { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string Phone { get; set; }
    }

    public class ApproveNgoRequestModel
    {
        public int NgoRequestId { get; set; }
    }

    public class RejectNgoRequestModel
    {
        public int NgoRequestId { get; set; }
    }

    public class NgoRequestModelProfile : Profile
    {
        public NgoRequestModelProfile()
        {
            CreateMap<Entities.NgoRequest, NgoRequestModel>()
                .ForMember(dest => dest.NgoRequestId, c => c.MapFrom(src => src.NgoRequestId))
                .ForMember(dest => dest.ContactPerson, c => c.MapFrom(src => src.ContactPerson))                
                .ForMember(dest => dest.Email, c => c.MapFrom(src => src.Email))
                .ForMember(dest => dest.NgoName, c => c.MapFrom(src => src.NgoName))
                .ForMember(dest => dest.Message, c => c.MapFrom(src => src.Message))
                .ForMember(dest => dest.RequestDate, c => c.MapFrom(src => src.RequestDate.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.StatusUpdateDate, c => c.MapFrom(src => src.StatusUpdateDate != null ? ((DateTime) src.StatusUpdateDate).ToString("dd.MM.yyyy") : null))
                .ForMember(dest => dest.RequestStatus, c => c.MapFrom(src => src.RequestStatus))
                .ForMember(dest => dest.Phone, c => c.MapFrom(src => src.Phone));
        }
    }
}
