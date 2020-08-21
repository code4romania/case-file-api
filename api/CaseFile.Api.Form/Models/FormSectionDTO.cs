using AutoMapper;
using System.Collections.Generic;
using CaseFile.Entities;

namespace CaseFile.Api.Form.Models
{
    public class FormSectionDTO
    {
        public FormSectionDTO()
        {
            Questions = new List<QuestionDTO>();
        }
        //public string UniqueId { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<QuestionDTO> Questions { get; set; }
    }

    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.OptionsToQuestions, c => c.MapFrom(src => src.OptionsToQuestions));

            CreateMap<OptionToQuestion, OptionToQuestionDTO>()
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Option.Text))
                .ForMember(dest => dest.IsFreeText, c => c.MapFrom(src => src.Option.IsFreeText))
                .ForMember(dest => dest.IdOption, c => c.MapFrom(src => src.OptionToQuestionId));
        }
    }
}
