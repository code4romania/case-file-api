using MediatR;
using CaseFile.Api.Answer.Models;

namespace CaseFile.Api.Answer.Queries
{
    public class FormAnswersQuery : IRequest<PollingStationInfosDTO>
    {
        public int PollingStationId { get; set; }
        public int ObserverId { get; set; }
    }
}