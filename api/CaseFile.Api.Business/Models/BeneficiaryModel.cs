using CaseFile.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Models
{
    public class BeneficiaryModel
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public CivilStatus CivilStatus { get; set; }
        public int CountyId { get; set; }
        public int CityId { get; set; }
        public Gender Gender { get; set; }

        public ICollection<FormModel> Forms { get; set; }
        public ICollection<FamilyMemberModel> FamilyMembers { get; set; }
    }

    public class FormModel
    {
        public int FormId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int TotalQuestionsNo { get; set; }
        public int QuestionsAnsweredNo { get; set; }
        public string UserName { get; set; }
    }

    public class FamilyMemberModel
    {
        public int BeneficiaryId { get; set; }
        public string Name { get; set; }
    }
}
