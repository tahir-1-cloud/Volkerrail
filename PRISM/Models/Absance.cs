using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Absance
    {
        public long Id { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? LeaveTypeId { get; set; }
        public long? EmployeeId { get; set; }
        public string? Reason { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? RecordStatus { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
