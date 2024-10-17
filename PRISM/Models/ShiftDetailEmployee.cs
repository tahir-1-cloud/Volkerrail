using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class ShiftDetailEmployee
    {
        public long Id { get; set; }
        public long? ShiftId { get; set; }
        public long? EmployeeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? RecordStatus { get; set; }
        public string? EmployeeType { get; set; }
        public string? ContactNumber { get; set; }
        public string? JobTitle { get; set; }
    }
}
