using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class ShiftFilter
    {
        public int Id { get; set; }
        public string? Template { get; set; }
        public string? MachineStatus { get; set; }
        public string? MachineNumber { get; set; }
        public string? ShiftStatus { get; set; }
        public string? FromWeek { get; set; }
        public string? ToWeek { get; set; }
        public string? Filters { get; set; }
        public long? UserId { get; set; }
    }
}
