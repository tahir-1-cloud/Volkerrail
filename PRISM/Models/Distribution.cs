using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Distribution
    {
        public long Id { get; set; }
        public string? EmailAddress { get; set; }
        public int? TypeId { get; set; }
        public string? ActiveStatus { get; set; }
    }
}
