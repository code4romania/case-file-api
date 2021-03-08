using CaseFile.Entities;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Models
{
    public class NewNgoRequestModel
    {
        public string ContactPerson { get; set; }
        public string NgoName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Phone { get; set; }
    }
}
