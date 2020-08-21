using MediatR;
using System.Collections.Generic;
using CaseFile.Api.Answer.Models;
using System;

namespace CaseFile.Api.Answer.Commands
{
    public class CompleteazaRaspunsCommand : IRequest<int>
    {
        public CompleteazaRaspunsCommand()
        {
            Answers = new List<AnswerDTO>();
        }
        public int UserId { get; set; }
        public int FormId { get; set; }
        public DateTime CompletionDate { get; set; }
        public List<AnswerDTO> Answers { get; set; }

    }
}