namespace CaseFile.Api.Auth.Models
{
    public class AuthenticationResponseModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public bool first_login { get; set; }
        public string phone_verification_request { get; set; }
    }
}