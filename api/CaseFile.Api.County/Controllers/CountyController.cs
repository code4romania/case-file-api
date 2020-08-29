using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using CaseFile.Api.County.Models;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using CaseFile.Api.County.Commands;
using CaseFile.Api.County.Queries;

namespace CaseFile.Api.County.Controllers
{
    [Authorize]
    [Route("api/v1/county")]
    public class CountyController : Controller
    {
        private readonly IMediator _mediator;
        public CountyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(List<CountyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCountiesAsync()
        {
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
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCitiesAsync(int countyId)
        {
            var response = await _mediator.Send(new GetCities(countyId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(new ErrorModel { Message = response.Error });
        }

    }
}
