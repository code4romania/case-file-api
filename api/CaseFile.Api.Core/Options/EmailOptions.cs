using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Core.Options
{
    public class EmailOptions
    {
        public string SendGridApiKey { get; set; }
        public string SupportEmail { get; set; }
        public string SupportName { get; set; }
    }
}
