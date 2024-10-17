using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class IndividualShift
    {
        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public string? Description { get; set; }
        public DateTime? PlannedStart { get; set; }
        public DateTime? PlannedFinish { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualFinish { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? RecordStatus { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
