using System.ComponentModel.DataAnnotations;

namespace CaseFile.Api.Business.Models
{
    public class ResetModel
    {
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}