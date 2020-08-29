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

namespace CaseFile.Api.Business.Controllers
{
    [Authorize]
    [Route("api/v1/beneficiary")]
    public class BeneficiaryController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly DefaultNgoOptions _defaultNgoOptions;

        //private int NgoId => this.GetIdOngOrDefault(_defaultNgoOptions.DefaultNgoId);
        private int UserId => this.GetCurrentUserId();
        

        public BeneficiaryController(IMediator mediator, IMapper mapper, IOptions<DefaultNgoOptions> defaultNgoOptions)
        {
            _mediator = mediator;
            _mapper = mapper;
            _defaultNgoOptions = defaultNgoOptions.Value;
        }

        [HttpGet]
        [Produces(type: typeof(ApiListResponse<BeneficiarySummaryModel>))]
        public async Task<ApiListResponse<BeneficiarySummaryModel>> GetBeneficiaries(BeneficiariesListQuery query)
        {
            var command = _mapper.Map<BeneficiariesListCommand>(query);

            if (query.UserId > 0)
                command.UserId = query.UserId;
            else
            {
                command.UserId = UserId;
                command.AllFromNgo = true;
            }

            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet]
        [Produces(type: typeof(ApiListResponse<BeneficiaryDetailsModel>))]
        [Route("details")]
        public async Task<ApiListResponse<BeneficiaryDetailsModel>> GetBeneficiariesWithDetails(BeneficiariesListQuery query)
        {
            var command = _mapper.Map<BeneficiariesDetailsListCommand>(query);

            command.UserId = UserId;

            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet]
        [Produces(type: typeof(int))]
        [Route("count")]
        public async Task<int> GetTotalBeneficiariesCount()
        {
            var result = await _mediator.Send(new BeneficiariesCountCommand { UserId = UserId });
            return result;
        }

        /// <summary>
        ///  Adds an beneficiary.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Id of the new beneficiary</returns>
        [HttpPost]
        [Produces(type: typeof(int))]
        public async Task<IActionResult> NewBeneficiary([FromBody]NewBeneficiaryModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<NewBeneficiaryCommand>(model);
            command.CurrentUserId = UserId;
            var newId = await _mediator.Send(command);

            return Ok(newId);
        }

        /// <summary>
        /// Edits beneficiary information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Boolean indicating whether or not the beneficiary was changed successfully</returns>
        [HttpPut]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> EditBeneficiary([FromBody]EditBeneficiaryModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var command = _mapper.Map<EditBeneficiaryCommand>(model);
            command.CurrentUserId = UserId;
            var id = await _mediator.Send(command);

            return Ok(id > 0);
        }

        [HttpGet("{beneficiaryId}")]
        [ProducesResponseType(typeof(BeneficiaryModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBeneficiaryAsync(int beneficiaryId)
        {
            var response = await _mediator.Send(new GetBeneficiary(beneficiaryId, UserId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(response.Error);
        }

        /// <summary>
        /// Deletes a beneficiary
        /// </summary>
        /// <param name="beneficiaryId">The Beneficiary id</param>
        /// <returns>Boolean indicating whether or not the beneficiary was deleted successfully</returns>
        [HttpDelete]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> DeleteBeneficiary(int beneficiaryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<DeleteBeneficiaryCommand>(new DeleteBeneficiaryModel { BeneficiaryId = beneficiaryId });
            command.UserId = UserId;

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost]
        [Produces(type: typeof(bool))]
        [Route("sendFile")]
        public async Task<IActionResult> SendFile(int beneficiaryId)
        {
            if (beneficiaryId <= 0)
                return BadRequest("Invalid beneficiary.");

            var command = _mapper.Map<SendBeneficiaryInfoCommand>(new SendBeneficiaryInfoModel { BeneficiaryId = beneficiaryId });
            command.UserId = UserId;

            var result = await _mediator.Send(command);

            return Ok(result.IsSuccess);
        }
        
    }
}