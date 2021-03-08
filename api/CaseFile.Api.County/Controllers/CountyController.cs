using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using CaseFile.Api.County.Models;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using CaseFile.Api.County.Queries;
using CaseFile.Api.Core;
using CaseFile.Api.Auth.Services;

namespace CaseFile.Api.County.Controllers
{
    [Authorize]
    [Route("api/v1/county")]
    public class CountyController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;

        public CountyController(IMediator mediator, ITokenService tokenService)
        {
            _mediator = mediator;
            _tokenService = tokenService;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(List<CountyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCountiesAsync()
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                return Unauthorized();

            var response = await _mediator.Send(new GetAllCounties());
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(new ErrorModel { Message = response.Error });
        }

        [HttpGet("{countyId}/cities")]
        [ProducesResponseType(typeof(List<CityModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCitiesAsync(int countyId)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                return Unauthorized();

            var response = await _mediator.Send(new GetCities(countyId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(new ErrorModel { Message = response.Error });
        }

    }
}
