using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class DriverQuestion
    {
        public long Id { get; set; }
        public bool? Question1 { get; set; }
        public bool? Question2 { get; set; }
        public bool? Question3 { get; set; }
        public bool? Question4 { get; set; }
        public bool? Question5 { get; set; }
        public bool? Question6 { get; set; }
        public bool? Question7 { get; set; }
        public bool? Question8 { get; set; }
        public string? Comments { get; set; }
        public string? IsSetisfied { get; set; }
        public long? DriverId { get; set; }
        public long? ShiftId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedId { get; set; }
        public bool? Question9 { get; set; }
        public bool? Question10 { get; set; }
    }
}
