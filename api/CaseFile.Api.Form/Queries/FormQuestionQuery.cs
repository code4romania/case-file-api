using MediatR;
using System.Collections.Generic;
using CaseFile.Api.Form.Models;

namespace CaseFile.Api.Form.Queries
{

    public class FormQuestionQuery : IRequest<IEnumerable<FormSectionDTO>>
    {
        public int UserId { get; set; }
        public int FormId { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }
}
