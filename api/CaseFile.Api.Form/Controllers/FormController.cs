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
using Microsoft.AspNetCore.Cors;

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

        private int UserId => this.GetIdObserver();        

        public FormController(IMediator mediator, IMapper mapper, IOptions<ApplicationCacheOptions> cacheOptions)
        {
            _cacheOptions = cacheOptions.Value;
            _mediator = mediator;
            _mapper = mapper;
        }

        //[EnableCors("_myAllowSpecificOrigins")]
        [HttpPost]
        [Produces(type: typeof(int))]
        public async Task<IActionResult> AddForm([FromBody]FormDTO newForm)
        {
            FormDTO result = await _mediator.Send(new AddFormQuery { Form = newForm, UserId = this.GetIdObserver() });
            return Ok(result.Id);
        }
        /// <summary>
        /// Returneaza versiunea tuturor formularelor sub forma unui array. 
        /// Daca versiunea returnata difera de cea din aplicatie, atunci trebuie incarcat formularul din nou 
        /// </summary>
        /// <returns></returns>
        [HttpGet("versions")]
        [Produces(typeof(Dictionary<string, int>))]
        public async Task<IActionResult> GetFormVersions()
        {
            var formsAsDict = new Dictionary<string, int>();
            (await _mediator.Send(new FormVersionQuery(null))).ForEach(form => formsAsDict.Add(form.Code, form.CurrentVersion));

            return Ok(new { Versions = formsAsDict });
        }

        /// <summary>
        /// Returns an array of forms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="formId">SectionId-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns></returns>
        [HttpGet("{formId}")]
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
    }
}