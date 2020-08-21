using MediatR;
using CaseFile.Api.Answer.Models;
using CaseFile.Api.Core;

namespace CaseFile.Api.Answer.Queries
{
    public class AnswersQuery : PagingModel, IRequest<ApiListResponse<AnswerQueryDTO>>
    {
        public int IdONG { get; set; }
        public bool Urgent { get; set; }
        public bool Organizer { get; set; }
        public string County { get; set; }
        public int PollingStationNumber { get; set; }
        public int ObserverId { get; set; }
    }
}