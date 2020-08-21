using AutoMapper;
using System;
using CaseFile.Api.Note.Commands;
using CaseFile.Api.Note.Models;

namespace CaseFile.Api.Note.Mapping
{
    public class NoteMappingProfile : Profile
    {
        public NoteMappingProfile()
        {
            CreateMap<UploadNoteModel, AddNoteCommand>(MemberList.Destination)
                .ForMember(dest => dest.QuestionId,
                    c => c.MapFrom(src => src.QuestionId == 0 || !src.QuestionId.HasValue ? null : src.QuestionId));

            CreateMap<AddNoteCommand, Entities.Note>(MemberList.Destination)
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BeneficiaryId, c => c.MapFrom(src => src.BeneficiaryId));
        }
    }
}