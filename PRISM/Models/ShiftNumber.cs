using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class ShiftNumber
    {
        public long Id { get; set; }
        public int ShiftNo { get; set; }
        public DateTime StartShiftDateTime { get; set; }
        public DateTime EndShiftDateTime { get; set; }
        public int? WeekNo { get; set; }
    }
}
