using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Week
    {
        public long Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Title { get; set; }
        public int? WeekNo { get; set; }
    }
}
