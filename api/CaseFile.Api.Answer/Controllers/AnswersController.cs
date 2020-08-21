using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseFile.Api.Answer.Commands;
using CaseFile.Api.Answer.Models;
using CaseFile.Api.Answer.Queries;
using CaseFile.Api.Core;

namespace CaseFile.Api.Answer.Controllers
{
    [Authorize]
    [Route("api/v1/answers")]
    public class AnswersController : Controller
    {
        private readonly IMediator _mediator;
        //private readonly IConfigurationRoot _configuration;

        public AnswersController(IMediator mediator) //, IConfigurationRoot configuration
        {
            _mediator = mediator;
            //_configuration = configuration;
        }

        ///// <summary>
        ///// Returns a list of polling stations where observers from the given NGO have submitted answers
        ///// to the questions marked as Flagged=Urgent, ordered by ModifiedDate descending
        ///// </summary>
        ///// <param name="model"> Pagination details(default Page=1, PageSize=20)
        ///// Urgent (Flagged)
        ///// </param>
        //[HttpGet]
        //public async Task<ApiListResponse<AnswerQueryDTO>> Get(SectionAnswersRequest model)
        //{
        //    //var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));
        //    var idOng = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));

        //    return await _mediator.Send(new AnswersQuery
        //    {
        //        IdONG = idOng,
        //        //Organizer = organizator,
        //        Page = model.Page,
        //        PageSize = model.PageSize,
        //        Urgent = model.Urgent,
        //        County = model.County,
        //        PollingStationNumber = model.PollingStationNumber,
        //        UserId = model.UserId
        //    });
        //}

        /// <summary>
        /// Returns answers given by the specified user for the specified beneficiary and form
        /// </summary>
        [HttpGet("filledIn")]
        public async Task<List<QuestionDTO<FilledInAnswerDTO>>> Get(int beneficiaryId, int userId, int? formId)
        {
            if (userId == 0)
                userId = this.GetIdObserver();

            return await _mediator.Send(new FilledInAnswersQuery
            {
                UserId = userId,
                BeneficiaryId = beneficiaryId,
                FormId = formId
            });
        }

        ///// <summary>
        ///// Returns the polling station information filled in by the given observer at the given polling station
        ///// </summary>
        ///// <param name="model"> "IdSectieDeVotare" - NgoId-ul sectiei unde s-au completat raspunsurile
        ///// "IdObservator" - NgoId-ul observatorului care a dat raspunsurile
        ///// </param>
        //[HttpGet("pollingStationInfo")]
        //public async Task<PollingStationInfosDTO> GetRaspunsuriFormular(ObserverAnswersRequest model)
        //{
        //    return await _mediator.Send(new FormAnswersQuery
        //    {
        //        UserId = model.UserId,
        //        BeneficiaryId = model.PollingStationNumber
        //    });
        //}


        /// <summary>
        /// Saves the answers to one or more questions, for a form for a beneficiary.
        /// An answer can have multiple options (OptionId) and potentially a free text (Value).
        /// </summary>
        /// <param name="answerModel">Beneficiary, list of options and the associated text of an option when
        /// <code>IsFreeText = true</code></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize("User")]
        public async Task<IActionResult> PostAnswer([FromBody] AnswerModelWrapper answerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO[DH] use a pipeline instead of separate Send commands
            var command = await _mediator.Send(new BulkAnswers(answerModel.Answers));

            command.UserId = this.GetIdObserver();
            command.CompletionDate = answerModel.CompletionDate;
            command.FormId = answerModel.FormId;

            var result = await _mediator.Send(command);

            if (result < 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}