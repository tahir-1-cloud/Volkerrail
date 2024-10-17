using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class ShiftTemplate
    {
        public int Id { get; set; }
        public string? TemplateName { get; set; }
        public string? Columns { get; set; }
        public string? UserId { get; set; }
    }
}
