using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Entities
{
    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class NgoRequest
    {
        public int NgoRequestId { get; set; }
        public string ContactPerson { get; set; }
        public string NgoName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? StatusUpdateDate { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string Phone { get; set; }

    }
}
