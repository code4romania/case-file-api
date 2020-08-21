using System;
using System.ComponentModel.DataAnnotations;

namespace CaseFile.Api.Note.Models
{
    public class NoteModel
    {
        //[Required(AllowEmptyStrings = false)]
        //public string CountyCode { get; set; }
        //[Required]
        //public int PollingStattionNumber { get; set; }
        public int? QuestionId { get; set; }
        public string Text { get; set; }
        public string AttachmentPath { get; set; }
        //public string FormCode { get; set; }
        //public int FormId { get; set; }
        public DateTime LastModified { get; set; }
    }
}
