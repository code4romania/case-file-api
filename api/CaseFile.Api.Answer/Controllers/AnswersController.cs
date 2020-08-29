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
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Server.IIS;
using System.Net;

namespace CaseFile.Api.Answer.Controllers
{
    [Authorize]
    [Route("api/v1/answers")]
    public class AnswersController : Controller
    {
        private readonly IMediator _mediator;

        public AnswersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns answers given by the specified user for the specified beneficiary and form
        /// </summary>
        [HttpGet("filledIn")]
        [ProducesResponseType(typeof(List<QuestionDTO<FilledInAnswerDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<List<QuestionDTO<FilledInAnswerDTO>>> Get(int beneficiaryId, int userId, int? formId)
        {
            if (beneficiaryId <= 0)
                throw new ArgumentException();

            if (userId == 0)
                userId = this.GetCurrentUserId();

            return await _mediator.Send(new FilledInAnswersQuery
            {
                UserId = userId,
                BeneficiaryId = beneficiaryId,
                FormId = formId
            });
        }

        /// <summary>
        /// Saves the answers to one or more questions, for a form for a beneficiary.
        /// An answer can have multiple options (OptionId) or a free text (Value).
        /// </summary>
        /// <param name="answerModel">Beneficiary, list of options and the associated text of an option when
        /// <code>IsFreeText = true</code></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAnswer([FromBody] AnswerModelWrapper answerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = await _mediator.Send(new BulkAnswers(answerModel.Answers));

            command.UserId = this.GetCurrentUserId();
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