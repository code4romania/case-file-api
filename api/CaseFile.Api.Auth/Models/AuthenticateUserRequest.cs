using System.ComponentModel.DataAnnotations;

namespace CaseFile.Api.Auth.Models
{
    public class AuthenticateUserRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}