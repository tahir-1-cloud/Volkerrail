using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class LogTypeEntry
    {
        public int Id { get; set; }
        public string? LogType { get; set; }
        public string? LogName { get; set; }
        public string? Description { get; set; }
    }
}
