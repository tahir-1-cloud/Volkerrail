using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class WeeklyComment
    {
        public long Id { get; set; }
        public long? WeekId { get; set; }
        public string? EngineeringSupport { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UserId { get; set; }
        public string? CoursesAndOthers { get; set; }
    }
}
