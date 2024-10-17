using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Route
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool? StablingPoints { get; set; }
        public string? Comments { get; set; }
        public string? ShortCode { get; set; }
        public int? Stanox { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? RecordStatus { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
