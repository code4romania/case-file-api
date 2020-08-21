namespace CaseFile.Api.Auth.Models
{
    public class RegisteredUserModel
    {
        public bool IsAuthenticated { get; set; }

        public int UserId { get; set; }

        public bool FirstAuthentication { get; set; }
        public int NgoId { get; set; }
        public string Phone { get; set; }
    }
}