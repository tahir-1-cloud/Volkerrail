using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class RoleModule
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsInsert { get; set; }
    }
}
