using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class MileStoneEntry
    {
        public long Id { get; set; }
        public long? ShiftId { get; set; }
        public string? LogEntry { get; set; }
        public string? MileStoneEntryDetail { get; set; }
        public string? Planned { get; set; }
        public string? Actuall { get; set; }
        public string? Comments { get; set; }
        public string? RecordStatus { get; set; }
    }
}
