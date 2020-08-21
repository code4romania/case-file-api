using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CaseFile.Api.Core.Attributes;

namespace CaseFile.Api.Note.Models
{
    public class UploadNoteModel
    {
        [Required]
        public int BeneficiaryId { get; set; }

        public int? QuestionId { get; set; }

        public string Text { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
}