using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class ShiftContact
    {
        public long Id { get; set; }
        public string? ContactType { get; set; }
        public long? ContactId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ShiftId { get; set; }
    }
}
