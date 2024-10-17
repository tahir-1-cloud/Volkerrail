using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Module
    {
        public int Id { get; set; }
        public string? ModuleName { get; set; }
        public string? ModuleUrl { get; set; }
        public string? ModuleId { get; set; }
    }
}
