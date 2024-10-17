using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class UserLog
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? AppName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ActionType { get; set; }
        public string? RecordStatus { get; set; }
        public string? Description { get; set; }
        public long? ShiftId { get; set; }
    }
}
