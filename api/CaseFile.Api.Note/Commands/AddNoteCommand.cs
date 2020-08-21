using MediatR;

namespace CaseFile.Api.Note.Commands
{
    public class AddNoteCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public int? QuestionId { get; set; }
        public string Text { get; set; }
        public string AttachementPath { get; set; }
    }
}
