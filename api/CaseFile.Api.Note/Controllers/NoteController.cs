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

namespace CaseFile.Api.Note.Controllers
{
    [Authorize]
    [Route("api/v2/note")]
    public class NoteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly FileServiceOptions _localFileOptions;

        public NoteController(IMediator mediator, IMapper mapper, IOptions<FileServiceOptions> options)
        {
            _mediator = mediator;
            _mapper = mapper;
            _localFileOptions = options.Value;
        }


        [HttpGet]
        public async Task<List<NoteModel>> Get(NoteQuery filter)
        {
            if (!filter.UserId.HasValue)
            {
                filter.UserId = this.GetIdObserver();
            }

            return await _mediator.Send(filter);
        }
        /// <summary>
        /// Aceasta ruta este folosita cand observatorul incarca o imagine sau un clip in cadrul unei note.
        /// Fisierului atasat i se da contenttype = Content-Type: multipart/form-data
        /// Celalalte proprietati sunt de tip form-data
        /// CodJudet:BU 
        /// NumarSectie:3243
        /// IdIntrebare: 201
        /// TextNota: "asdfasdasdasdas"
        /// API-ul va returna adresa publica a fisierului unde este salvat si obiectul trimis prin formdata
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<dynamic> Upload([FromForm]UploadNoteModel note)
        {
            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest);
            }
            
            // todo: check if beneficiary exists

            var command = _mapper.Map<AddNoteCommand>(note);
            string fileAddress = null;
            if (note.File != null)
                fileAddress = await _mediator.Send(new UploadFileCommand { File = note.File, UploadType = UploadType.Notes });
            
            command.UserId = this.GetIdObserver();
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
        [AllowAnonymous]
        public async Task<IActionResult> Upload(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                var fullPath = _localFileOptions.StoragePaths[UploadType.Notes.ToString()] + "\\" + filename; //"\\notes\\" + filename; /* "\\home\\site\\wwwroot\\notes" */
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