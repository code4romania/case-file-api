using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CaseFile.Entities
{
    public enum CivilStatus
    {
        NotMarried,
        Married,
        Divorced,
        Widowed
    }

    //public enum FamilyRole
    //{
    //    Child,
    //    Partner,
    //    Parent
    //}

    public enum Gender
    {
        Male,
        Female
    }

    public partial class Beneficiary
    {
        public int BeneficiaryId { get; set; }
        public int? FamilyId { get; set; }
        public int UserId { get; set; }
        public int? CityId { get; set; }
        public int? CountyId { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public string Address { get; set; }        
        public CivilStatus CivilStatus { get; set; }
        public string PictureUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public DateTime RegistrationDate { get; set; }

        public virtual County County { get; set; }
        public virtual City City { get; set; }
        public virtual Family Family { get; set; }
        public virtual User User { get; set; }  // Assigned assistant or Ngo admin
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<UserForm> UserForms { get; set; }
    }
}
