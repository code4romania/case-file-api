using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseFile.Entities
{
    public enum Type
    {
        CommunityAssistant,
        SocialWorker,
        HealthMediator,
        Pediatrician,
        Gynecologist,
        Psychologist,
        SpeechTherapist,
        Other
    }

    public enum Role
    {
        Assistant,
        NgoAdmin,
        Admin
    }

    public partial class User
    {
        public int UserId { get; set; }        
        public int NgoId { get; set; }
        public int? CreatedByUserId { get; set; }
        public string Email { get; set; }
        [MaxLength(100)]
        public string Password { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }        
        [MaxLength(20)]
        public string Phone { get; set; }
        public string PictureUrl { get; set; }
        public Role Role { get; set; }
        public string TypeList { get; set; } // list of Type, separated by ','
        public DateTime BirthDate { get; set; }
        public DateTime? LastSignIn { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        //public bool EmailConfirmed { get; set; }
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }
        public string TemporaryToken { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }        
        public virtual Ngo Ngo { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserForm> UserForms { get; set; }
    }
}
