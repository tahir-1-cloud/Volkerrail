using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class LeaveType
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? ColorCode { get; set; }
    }
}
