using CaseFile.Entities;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Models
{
    public class NewUserModel
    {
        public int? NgoId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Role Role { get; set; }
        public DateTime BirthDate { get; set; }
        public List<int> Types { get; set; }
    }
}
