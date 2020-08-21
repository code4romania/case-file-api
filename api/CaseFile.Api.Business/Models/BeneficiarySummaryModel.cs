using AutoMapper;
using CaseFile.Entities;
using System;

namespace CaseFile.Api.Business.Models
{
    public class BeneficiarySummaryModel
    {
        public int BeneficiaryId { get; set; }        
        public string Name { get; set; }        
        public int Age { get; set; }
        public CivilStatus CivilStatus { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public Gender Gender { get; set; }
        public string AssistantName { get; set; }
        public string RegistrationDate { get; set; }
    }

    public class BeneficiaryModelProfile : Profile
    {
        public BeneficiaryModelProfile()
        {
            CreateMap<Entities.Beneficiary, BeneficiarySummaryModel>()
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Age, c => c.MapFrom(src => (int)((DateTime.UtcNow - src.BirthDate).TotalDays / 365.2425)))
                .ForMember(dest => dest.CivilStatus, c => c.MapFrom(src => src.CivilStatus))
                .ForMember(dest => dest.County, c => c.MapFrom(src => src.County.Name))
                .ForMember(dest => dest.City, c => c.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.Gender, c => c.MapFrom(src => src.Gender))
                .ForMember(dest => dest.AssistantName, c => c.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.RegistrationDate, c => c.MapFrom(src => src.RegistrationDate.ToString("dd.MM.yyyy"))); 

            CreateMap<Entities.Beneficiary, BeneficiaryDetailsModel>()
                .ForMember(dest => dest.BeneficiaryId, c => c.MapFrom(src => src.BeneficiaryId))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Age, c => c.MapFrom(src => (int)((DateTime.UtcNow - src.BirthDate).TotalDays / 365.2425)))
                .ForMember(dest => dest.CivilStatus, c => c.MapFrom(src => src.CivilStatus))
                .ForMember(dest => dest.County, c => c.MapFrom(src => src.County.Name))
                .ForMember(dest => dest.City, c => c.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.Gender, c => c.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, c => c.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.CountyId, c => c.MapFrom(src => src.CountyId))
                .ForMember(dest => dest.CityId, c => c.MapFrom(src => src.CityId))
                .ForMember(dest => dest.UserId, c => c.MapFrom(src => src.UserId));
        }
    }
}
