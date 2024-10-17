using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class AppUser
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? EmailAddress { get; set; }
        public string? RecordStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? RoleId { get; set; }
        public string? FullName { get; set; }
    }
}
