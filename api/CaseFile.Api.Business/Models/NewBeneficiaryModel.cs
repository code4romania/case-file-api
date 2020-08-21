using CaseFile.Entities;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Models
{
    public class NewBeneficiaryModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public CivilStatus CivilStatus { get; set; }        
        public int CountyId { get; set; }
        public int CityId { get; set; }
        public Gender Gender { get; set; }
        public int? IsFamilyOfBeneficiaryId { get; set; }
        public ICollection<int> FormsIds { get; set; }
        public ICollection<int> FamilyMembersIds { get; set; }
    }
}
