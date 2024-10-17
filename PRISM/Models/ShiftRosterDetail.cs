using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class ShiftRosterDetail
    {
        public long Id { get; set; }
        public long? ShiftId { get; set; }
        public string? RosterShiftDescription { get; set; }
    }
}
