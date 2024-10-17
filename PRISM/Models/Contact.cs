using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Contact
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? FaxNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Notes { get; set; }
        public string? Email { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? RecordStatus { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
