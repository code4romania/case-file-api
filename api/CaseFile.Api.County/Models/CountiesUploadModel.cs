using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CaseFile.Api.Core.Attributes;

namespace CaseFile.Api.County.Models
{
    public class CountiesUploadModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".csv" })]
        public IFormFile CsvFile { get; set; }
    }
}