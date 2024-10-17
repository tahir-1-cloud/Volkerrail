using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class MachineStatus
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
    }
}
