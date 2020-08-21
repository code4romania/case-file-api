using CaseFile.Entities;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Models
{
    public class EditBeneficiaryModel
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public CivilStatus CivilStatus { get; set; }
        public Gender Gender { get; set; }
        public int CountyId { get; set; }
        public int CityId { get; set; }
        public ICollection<int> NewAllocatedFormsIds { get; set; }
        public ICollection<int> DealocatedFormsIds { get; set; }
        public ICollection<int> FamilyMembersIds { get; set; }
    }
}
