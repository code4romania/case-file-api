using AutoMapper;
using System.Linq;
using CaseFile.Entities;

namespace CaseFile.Api.Answer.Models
{
    public class FilledInAnswerDTO
    {
        public int IdOption { get; set; }
        public string Text { get; set; }
        public bool IsFreeText { get; set; }
        public string Value { get; set; }
        //public bool Flagged { get; set; }
    }

    public class AnswerProfile : Profile
    {
        public AnswerProfile()
        {
            CreateMap<Question, QuestionDTO<FilledInAnswerDTO>>()
                .ForMember(dest => dest.Answers, c => c.MapFrom(src => src.OptionsToQuestions))
                .ForMember(dest => dest.QuestionId, c => c.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text))
                .ForMember(dest => dest.IdQuestionType, c => c.MapFrom(src => src.QuestionType))
                .ForMember(dest => dest.Code, c => c.MapFrom(src => src.Code))
                .ForMember(dest => dest.FormId, c => c.MapFrom(src => src.FormSection.FormId))
                //.ForMember(dest => dest.IsMandatory, c => c.MapFrom(src => src.IsMandatory))
                //.ForMember(dest => dest.CharsNo, c => c.MapFrom(src => src.CharsNo))
                ;

            CreateMap<OptionToQuestion, FilledInAnswerDTO>()
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Option.Text))
                .ForMember(dest => dest.IsFreeText, c => c.MapFrom(src => src.Option.IsFreeText))
                .ForMember(dest => dest.IdOption, c => c.MapFrom(src => src.OptionToQuestionId))
                //.ForMember(dest => dest.Flagged, c => c.MapFrom(src => src.Flagged))
                .ForMember(dest => dest.Value, c => c.MapFrom(src => src.Answers.First().Value));
        }
    }
}
