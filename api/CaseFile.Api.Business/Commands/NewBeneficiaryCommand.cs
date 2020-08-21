using AutoMapper;
using MediatR;
using CaseFile.Api.Business.Models;
using System;
using CaseFile.Entities;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Commands
{
    public class NewBeneficiaryCommand : IRequest<int>
    {
        public int CurrentUserId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public CivilStatus CivilStatus { get; set; }
        public Gender Gender { get; set; }
        public int CountyId { get; set; }
        public int CityId { get; set; }
        public int? IsFamilyOfBeneficiaryId { get; set; }
        public ICollection<int> FormsIds { get; set; }
        public ICollection<int> FamilyMembersIds { get; set; }
    }
    public class BeneficiaryProfile : Profile
    {
        public BeneficiaryProfile()
        {
            CreateMap<NewBeneficiaryModel, NewBeneficiaryCommand>()
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.BirthDate, c => c.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.CivilStatus, c => c.MapFrom(src => src.CivilStatus))
                .ForMember(dest => dest.CountyId, c => c.MapFrom(src => src.CountyId))
                .ForMember(dest => dest.CityId, c => c.MapFrom(src => src.CityId))
                .ForMember(dest => dest.Gender, c => c.MapFrom(src => src.Gender))
                .ForMember(dest => dest.FormsIds, c => c.MapFrom(src => src.FormsIds))
                .ForMember(dest => dest.IsFamilyOfBeneficiaryId, c => c.MapFrom(src => src.IsFamilyOfBeneficiaryId))
                .ForMember(dest => dest.UserId, c => c.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FamilyMembersIds, c => c.MapFrom(src => src.FamilyMembersIds))
               ;

            CreateMap<EditBeneficiaryModel, EditBeneficiaryCommand>()
                .ForMember(dest => dest.BeneficiaryId, c => c.MapFrom(src => src.BeneficiaryId))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.BirthDate, c => c.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.CivilStatus, c => c.MapFrom(src => src.CivilStatus))
                .ForMember(dest => dest.CountyId, c => c.MapFrom(src => src.CountyId))
                .ForMember(dest => dest.CityId, c => c.MapFrom(src => src.CityId))
                .ForMember(dest => dest.Gender, c => c.MapFrom(src => src.Gender))
                .ForMember(dest => dest.NewAllocatedFormsIds, c => c.MapFrom(src => src.NewAllocatedFormsIds))
                .ForMember(dest => dest.DealocatedFormsIds, c => c.MapFrom(src => src.DealocatedFormsIds))
                .ForMember(dest => dest.FamilyMembersIds, c => c.MapFrom(src => src.FamilyMembersIds))
                .ForMember(dest => dest.UserId, c => c.MapFrom(src => src.UserId))
               ;

            CreateMap<DeleteBeneficiaryModel, DeleteBeneficiaryCommand>()
                .ForMember(dest => dest.BeneficiaryId, c => c.MapFrom(src => src.BeneficiaryId))
               ;

            CreateMap<SendBeneficiaryInfoModel, SendBeneficiaryInfoCommand>()
                .ForMember(dest => dest.BeneficiaryId, c => c.MapFrom(src => src.BeneficiaryId))
               ;
        }
    }

}
