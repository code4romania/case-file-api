using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CaseFile.Api.Core;
using CaseFile.Api.Core.Commands;
using CaseFile.Api.Note.Commands;
using CaseFile.Api.Note.Models;
using CaseFile.Api.Note.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using CaseFile.Api.Core.Options;
using Microsoft.Extensions.Options;
using CaseFile.Api.Auth.Services;
using System;

namespace CaseFile.Api.Note.Controllers
{
    [Authorize]
    [Route("api/v2/note")]
    public class NoteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly FileServiceOptions _localFileOptions;
        private readonly ITokenService _tokenService;

        public NoteController(IMediator mediator, IMapper mapper, IOptions<FileServiceOptions> options, ITokenService tokenService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _localFileOptions = options.Value;
            _tokenService = tokenService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<NoteModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<List<NoteModel>> Get(NoteQuery filter)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                throw new UnauthorizedAccessException();

            if (!filter.UserId.HasValue || filter.UserId <= 0)
            {
                filter.UserId = this.GetCurrentUserId();
            }

            return await _mediator.Send(filter);
        }
        /// <summary>
        /// Aceasta ruta este folosita cand userul incarca o imagine sau un clip in cadrul unei note.
        /// Fisierului atasat i se da contenttype = Content-Type: multipart/form-data
        /// Celalalte proprietati sunt de tip form-data        
        /// API-ul va returna adresa publica unde e salvat fisierul
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<dynamic> Upload([FromForm]UploadNoteModel note)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                throw new UnauthorizedAccessException();

            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest);
            }
            
            var command = _mapper.Map<AddNoteCommand>(note);
            string fileAddress = null;
            if (note.File != null)
                fileAddress = await _mediator.Send(new UploadFileCommand { File = note.File, UploadType = UploadType.Notes });
            
            command.UserId = this.GetCurrentUserId();
            command.AttachementPath = Request.GetDisplayUrl() + "?filename=" + fileAddress;
            command.BeneficiaryId = note.BeneficiaryId;

            var result = await _mediator.Send(command);

            if (result < 0)
            {
                return this.ResultAsync(HttpStatusCode.NotFound);
            }

            return await Task.FromResult(new { FileAddress = command.AttachementPath });
        }

        // GET
        [HttpGet("upload")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upload(string filename)
        {
            if (!this.IsTokenValid(_tokenService.GetTemporaryToken(this.GetCurrentUserId())))
                throw new UnauthorizedAccessException();

            if (!string.IsNullOrEmpty(filename))
            {
                var fullPath = _localFileOptions.StoragePaths[UploadType.Notes.ToString()] + "\\" + filename;
                if (System.IO.File.Exists(fullPath))
                {
                    Stream stream = System.IO.File.OpenRead(fullPath);

                    if (stream == null)
                        return NotFound();

                    var provider = new FileExtensionContentTypeProvider();
                    string contentType;
                    if (!provider.TryGetContentType(filename, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    return File(stream, contentType);
                }
            }

            return NotFound();
        }
    }
}