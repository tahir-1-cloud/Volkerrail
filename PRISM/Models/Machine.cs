using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Machine
    {
        public long Id { get; set; }
        public string? Number { get; set; }
        public string? Type { get; set; }
        public int? CategoryId { get; set; }
        public string? OwnerName { get; set; }
        public string? ManagerName { get; set; }
        public string? Area { get; set; }
        public string? Description { get; set; }
        public string? HeadCode { get; set; }
        public string? Specification { get; set; }
        public decimal? Speed { get; set; }
        public decimal? Weight { get; set; }
        public string? Capabilities { get; set; }
        public string? Nrn1 { get; set; }
        public string? Nrn2 { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? RecordStatus { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
