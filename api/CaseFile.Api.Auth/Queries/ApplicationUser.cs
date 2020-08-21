using MediatR;
using System.ComponentModel.DataAnnotations;
using CaseFile.Api.Auth.Models;

namespace CaseFile.Api.Auth.Queries
{
    /// <summary>
    /// Model received from client applications in order to perform the authentication
    /// </summary>
    public class ApplicationUser : IRequest<RegisteredUserModel>
    {
        /// <summary>
        /// Email's phone number
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        //[DataType(DataType.PhoneNumber)]
        public string Email { get; set; }

        /// <summary>
        /// PIN number used for authentication (should have received this number by SMS)
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        ///// <summary>
        ///// This is the unique identifier of the mobile device
        ///// </summary>
        //[Required(AllowEmptyStrings = false)]
        //public string UDID { get; set; }
    }
}
