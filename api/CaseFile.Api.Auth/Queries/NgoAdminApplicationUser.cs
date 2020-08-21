using MediatR;
using System.ComponentModel.DataAnnotations;
using CaseFile.Api.Auth.Models;
using CaseFile.Api.Core.Models;

namespace CaseFile.Api.Auth.Queries
{
    public class NgoAdminApplicationUser : IRequest<UserInfo>
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        public UserType UserType { get; set; }
    }
}
