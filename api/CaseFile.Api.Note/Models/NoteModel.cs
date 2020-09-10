using System;
using System.ComponentModel.DataAnnotations;

namespace CaseFile.Api.Note.Models
{
    public class NoteModel
    {        
        public int? QuestionId { get; set; }
        public string Text { get; set; }
        public string AttachmentPath { get; set; }
        public DateTime LastModified { get; set; }
        //public string AssistantName { get; set; }
    }
}
