using MediatR;
using System.Collections.Generic;
using CaseFile.Api.Form.Models;
using CaseFile.Api.Core;
using AutoMapper;

namespace CaseFile.Api.Form.Queries
{
    public class FormVersionQuery : IRequest<List<FormDetailsModel>>
    {
        public int? UserId { get; }

        public FormVersionQuery(int? userId)
        {
            UserId = userId;
        }
    }

    public class FormQuestionsQuery : IRequest<IEnumerable<ModelSectiune>>
    {
        public string CodFormular { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }

    public class FormListQuery : PagingModel
    {
        public string Description { get; set; }
        public int? UserId { get; set; }
    }

    public class FormListCommand : IRequest<ApiListResponse<FormResultModel>>
    {
        public int UserId { get; set; }
        public string Description { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class DeleteFormModel
    {
        public int FormId { get; set; }
    }

    public class DeleteFormCommand : IRequest<bool>
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
    }

    public class PublishFormModel
    {
        public int FormId { get; set; }
    }

    public class PublishFormCommand : IRequest<bool>
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
    }

    public class FormListCommandProfile : Profile
    {
        public FormListCommandProfile()
        {
            CreateMap<FormListQuery, FormListCommand>()
                .ForMember(dest => dest.Description, c => c.MapFrom(src => src.Description))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;

            CreateMap<DeleteFormModel, DeleteFormCommand>()
                .ForMember(dest => dest.FormId, c => c.MapFrom(src => src.FormId))
               ;
        }
    }
}
