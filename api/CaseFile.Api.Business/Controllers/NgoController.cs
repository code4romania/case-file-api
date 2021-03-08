using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseFile.Api.Core;
using CaseFile.Api.Core.Commands;
using CaseFile.Api.Core.Options;
using CaseFile.Api.Business.Commands;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using CaseFile.Api.Auth.Services;
using System;

namespace CaseFile.Api.Business.Controllers
{
    [Authorize]
    [Route("api/v1/ngo")]
    public class NgoController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly DefaultNgoOptions _defaultNgoOptions;
        private readonly ITokenService _tokenService;

        private int NgoId => this.GetIdOngOrDefault(_defaultNgoOptions.DefaultNgoId);

        private int UserId => this.GetCurrentUserId();

        public NgoController(IMediator mediator, IMapper mapper, IOptions<DefaultNgoOptions> defaultNgoOptions, ITokenService tokenService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _defaultNgoOptions = defaultNgoOptions.Value;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Produces(type: typeof(ApiListResponse<NgoRequestModel>))]
        public async Task<ApiListResponse<NgoRequestModel>> GetNgoRequests(NgoRequestsListQuery query)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                throw new UnauthorizedAccessException();

            var command = _mapper.Map<NgoRequestsListCommand>(query);

            command.UserId = UserId;

            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        ///  Adds a ngo request.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The id of the new ngo request.</returns>
        [HttpPost]
        [Produces(type: typeof(int))]
        [AllowAnonymous]
        public async Task<IActionResult> NewNgoRequest([FromBody] NewNgoRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newNgoRequestCommand = _mapper.Map<NewNgoRequestCommand>(model);
            
            var newId = await _mediator.Send(newNgoRequestCommand);

            return Ok(newId);
        }

        /// <summary>
        /// Approve NGO request.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>Boolean indicating whether or not the request was successfully approved.</returns>
        [HttpPost]
        [Produces(type: typeof(bool))]
        [Route("approve")]
        public async Task<IActionResult> ApproveNgoRequest([FromBody]int requestId)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<ApproveNgoRequestCommand>(new ApproveNgoRequestModel { NgoRequestId = requestId });
            command.UserId = UserId;

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Reject NGO request.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>Boolean indicating whether or not the request was successfully rejected.</returns>
        [HttpPost]
        [Produces(type: typeof(bool))]
        [Route("reject")]
        public async Task<IActionResult> RejectNgoRequest([FromBody] int requestId)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<RejectNgoRequestCommand>(new RejectNgoRequestModel { NgoRequestId = requestId });
            command.UserId = UserId;

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}