using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class FilterHistory
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? Template { get; set; }
        public string? FromWeek { get; set; }
        public string? ToWeek { get; set; }
        public bool? Otm { get; set; }
        public string? MachineStatus { get; set; }
        public string? ShiftStatus { get; set; }
        public string? MachineNumber { get; set; }
        public bool? Otpm { get; set; }
        public bool? Ott { get; set; }
        public bool? Act { get; set; }
        public bool? ShiftBlank { get; set; }
        public bool? ShiftCaped { get; set; }
        public bool? ShiftCancelled { get; set; }
        public bool? Owner { get; set; }
    }
}
