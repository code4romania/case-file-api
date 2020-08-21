using AutoMapper;
using CaseFile.Entities;
using System;
using System.Collections.Generic;

namespace CaseFile.Api.Business.Models
{
    public class BeneficiaryDetailsModel
    {
        public int BeneficiaryId { get; set; }        
        public string Name { get; set; }        
        public int Age { get; set; }
        public CivilStatus CivilStatus { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public Gender Gender { get; set; }
        public int UserId { get; set; }        
        public DateTime BirthDate { get; set; }
        public int CountyId { get; set; }
        public int CityId { get; set; }

        public ICollection<FormModel> Forms { get; set; }
        public ICollection<FamilyMemberModel> FamilyMembers { get; set; }
    }
}
