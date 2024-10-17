using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class Employee
    {
        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Initials { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? RecordStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public int? EmployeeTypeId { get; set; }
        public string? EmployeeId { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string? Manager { get; set; }
        public string? ReportsTo { get; set; }
        public string? Location { get; set; }
        public string? Gang { get; set; }
        public string? ContactNumber { get; set; }
    }
}
