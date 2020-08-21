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
        private int UserId => this.GetIdObserver();
        

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

            // TODO!!! if model.UserId ! = user.Id && user.Role == asistent throw not allowed operation error
            // if allowed then set the assistent of the new beneficiary to model.UserId
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

            // TODO!!! if model.UserId ! = user.Id && user.Role == asistent throw not allowed operation error
            // if allowed then set the assistent of the new beneficiary to model.UserId
            var command = _mapper.Map<EditBeneficiaryCommand>(model);
            command.CurrentUserId = UserId;
            var id = await _mediator.Send(command);

            return Ok(id > 0);
        }

        [HttpGet("{beneficiaryId}")]
        [ProducesResponseType(typeof(BeneficiaryModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBeneficiaryAsync(int beneficiaryId)
        {
            var response = await _mediator.Send(new GetBeneficiary(beneficiaryId));
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
            // check that the logged in user is allowed to send the file -> the user is the beneficiary's assistent
            // or a member of the ngo where the beneficiary is registered

            if (beneficiaryId <= 0)
                return BadRequest("Invalid beneficiary.");

            var command = _mapper.Map<SendBeneficiaryInfoCommand>(new SendBeneficiaryInfoModel { BeneficiaryId = beneficiaryId });
            command.UserId = UserId;

            var result = await _mediator.Send(command);

            return Ok(result.IsSuccess);
        }

        //[HttpPost]
        //[Route("reset")]
        //[Authorize("NgoAdmin")]
        //public async Task<IActionResult> Reset([FromBody]ResetModel model)
        //{
        //    if (string.IsNullOrEmpty(model.Action) || string.IsNullOrEmpty(model.PhoneNumber))
        //    {
        //        return BadRequest();
        //    }

        //    if (string.Equals(model.Action, ControllerExtensions.DEVICE_RESET))
        //    {
        //        var result = await _mediator.Send(new ResetDeviceCommand
        //        {
        //            NgoId = NgoId,
        //            PhoneNumber = model.PhoneNumber,
        //            //Organizer = this.GetOrganizatorOrDefault(false)
        //        });
        //        if (result == -1)
        //        {
        //            return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);
        //        }
        //        else
        //        {
        //            return Ok(result);
        //        }
        //    }

        //    if (string.Equals(model.Action, ControllerExtensions.PASSWORD_RESET))
        //    {
        //        var result = await _mediator.Send(new ResetPasswordCommand
        //        {
        //            NgoId = NgoId,
        //            PhoneNumber = model.PhoneNumber,
        //            Pin = model.Pin,
        //            //Organizer = this.GetOrganizatorOrDefault(false)
        //        });
        //        if (result == false)
        //        {
        //            return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);
        //        }

        //        return Ok();
        //    }

        //    return UnprocessableEntity();
        //}

        //[HttpPost]
        //[Route("generate")]
        //[Produces(type: typeof(List<GeneratedObserver>))]
        //public async Task<IActionResult> GenerateObservers([FromForm] int count)
        //{
        //    if (!ControllerExtensions.ValidateGenerateObserversNumber(count))
        //    {
        //        return BadRequest("Incorrect parameter supplied, please check that paramter is between boundaries: "
        //            + ControllerExtensions.LOWER_OBS_VALUE + " - " + ControllerExtensions.UPPER_OBS_VALUE);
        //    }

        //    var command = new ObserverGenerateCommand(count, NgoId);

        //    var result = await _mediator.Send(command);

        //    return Ok(result);
        //}
    }
}