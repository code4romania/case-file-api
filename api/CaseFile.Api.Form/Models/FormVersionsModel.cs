
using CaseFile.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CaseFile.Api.Form.Models
{
    public class FormVersionsModel
    {
        public List<FormDetailsModel> FormVersions { get; set; }
    }

    public class FormDetailsModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "ver")]
        public int CurrentVersion { get; set; }
        
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "user")]
        public string UserName { get; set; }
    }

    public class FormResultModel : FormDetailsModel
    {
        [JsonProperty(PropertyName = "canBeModified")]
        public bool CanBeModified { get; set; }
        [JsonProperty(PropertyName = "type")]
        public FormType Type { get; set; }
    }

}
