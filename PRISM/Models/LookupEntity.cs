using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class LookupEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LookupType { get; set; }
    }
}
