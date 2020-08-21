using AutoMapper;
using CaseFile.Entities;

namespace CaseFile.Api.Answer.Models
{
    public class AvailableAnswerDTO
    {
        public int IdOption { get; set; }
        public string Text { get; set; }
        public bool IsFreeText { get; set; }
    }

    public class FormularProfile : Profile
    {
        public FormularProfile()
        {
            CreateMap<Question, QuestionDTO<AvailableAnswerDTO>>()
                .ForMember(dest => dest.Answers, c => c.MapFrom(src => src.OptionsToQuestions))
                .ForMember(dest => dest.QuestionId, c => c.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text))
                .ForMember(dest => dest.IdQuestionType, c => c.MapFrom(src => src.QuestionType))
                .ForMember(dest => dest.Code, c => c.MapFrom(src => src.Code))
                .ForMember(dest => dest.FormId, c => c.MapFrom(src => src.FormSection.FormId))
                ;

            CreateMap<OptionToQuestion, AvailableAnswerDTO>()
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Option.Text))
                .ForMember(dest => dest.IsFreeText, c => c.MapFrom(src => src.Option.IsFreeText))
                .ForMember(dest => dest.IdOption, c => c.MapFrom(src => src.OptionToQuestionId));
        }
    }
}
