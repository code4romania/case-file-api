using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseFile.Api.Core.Options;
using CaseFile.Api.Form.Models;
using CaseFile.Api.Form.Queries;
using CaseFile.Api.Core;
using AutoMapper;

namespace CaseFile.Api.Form.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Ruta Formular ofera suport pentru toate operatiile legate de formularele completate de observatori
    /// </summary>
    [Authorize]
    [Route("api/v1/form")]
    public class FormController : Controller
    {
        private readonly ApplicationCacheOptions _cacheOptions;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        private int UserId => this.GetCurrentUserId();        

        public FormController(IMediator mediator, IMapper mapper, IOptions<ApplicationCacheOptions> cacheOptions)
        {
            _cacheOptions = cacheOptions.Value;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Produces(type: typeof(int))]
        public async Task<IActionResult> AddForm([FromBody]FormDTO newForm)
        {
            FormDTO result = await _mediator.Send(new AddFormQuery { Form = newForm, UserId = this.GetCurrentUserId() });
            return Ok(result.Id);
        }
        
        /// <summary>
        /// Returns an array of forms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(type: typeof(FormVersionsModel))]
        public async Task<IActionResult> GetFormsAsync()
            => Ok(new FormVersionsModel { FormVersions = await _mediator.Send(new FormVersionQuery(UserId)) });

        [HttpGet("search")]
        [Produces(type: typeof(ApiListResponse<FormDetailsModel>))]
        public async Task<ApiListResponse<FormDetailsModel>> GetForms(FormListQuery query)
        {
            var command = _mapper.Map<FormListCommand>(query);
            command.UserId = UserId;

            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru users si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="formId">SectionId-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns></returns>
        [HttpGet("{formId}")]
        [Produces(type: typeof(IEnumerable<FormSectionDTO>))]
        public async Task<IEnumerable<FormSectionDTO>> GetFormAsync(int formId)
        {
            var result = await _mediator.Send(new FormQuestionQuery
            {
                UserId = UserId,
                FormId = formId,
                CacheHours = _cacheOptions.Hours,
                CacheMinutes = _cacheOptions.Minutes,
                CacheSeconds = _cacheOptions.Seconds
            });

            return result;
        }

        [HttpDelete]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> DeleteForm(int formId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<DeleteFormCommand>(new DeleteFormModel { FormId = formId });
            command.UserId = UserId;
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}