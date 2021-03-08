using AutoMapper;
using CaseFile.Api.Auth.Services;
using CaseFile.Api.Business.Commands;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using CaseFile.Api.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CaseFile.Api.Business.Controllers
{
    [Authorize]
    [Route("api/v1/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        private int UserId => this.GetCurrentUserId();

        public StatisticsController(IMediator mediator, IMapper mapper, ITokenService tokenService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Produces(type: typeof(ApiListResponse<ReportInfoModel>))]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ApiListResponse<ReportInfoModel>> GetReports(ReportListQuery query)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                throw new UnauthorizedAccessException();

            var command = _mapper.Map<ReportListCommand>(query);
            command.UserId = UserId;

            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        ///  Adds a report.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Id of the new report</returns>
        [HttpPost]
        [Produces(type: typeof(int))]
        public async Task<IActionResult> NewReport([FromBody]NewReportModel model)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<NewReportCommand>(model);
            command.CurrentUserId = UserId;
            var newId = await _mediator.Send(command);

            return Ok(newId);
        }

        [HttpGet("{reportId}")]
        [ProducesResponseType(typeof(ReportModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReportAsync(int reportId)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                return Unauthorized();

            var response = await _mediator.Send(new GetReport(reportId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(response.Error);
        }

        [HttpGet]
        [Route("info")]
        [ProducesResponseType(typeof(StatisticsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGeneralStatistics()
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                return Unauthorized();

            var response = await _mediator.Send(new GetGeneralStatistics(UserId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(response.Error);
        }
    }
}
