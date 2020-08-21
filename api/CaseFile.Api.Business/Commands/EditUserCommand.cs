using CaseFile.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Commands
{
    public class EditUserCommand : IRequest<int>
    {
        public int CurrentUserId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Role Role { get; set; }
        public DateTime BirthDate { get; set; }
        public List<int> Types { get; set; }
    }
}
