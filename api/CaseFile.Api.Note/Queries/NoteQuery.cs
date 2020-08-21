using MediatR;
using System.Collections.Generic;
using CaseFile.Api.Note.Models;

namespace CaseFile.Api.Note.Queries
{
    public class NoteQuery : IRequest<List<NoteModel>>
    {
        public int BeneficiaryId { get; set; }
        public int? UserId { get; set; }
        public int? FormId { get; set; }
    }
}