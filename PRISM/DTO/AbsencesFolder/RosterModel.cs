using PRISM.DTO.Employeedto;
using PRISM.Models;

namespace PRISM.DTO.AbsencesFolder
{
    public class RosterModel
    {
        public long Id { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? LeaveTypeId { get; set; }
        public long? EmployeeId { get; set; }
        public long? ShiftId { get; set; }
        public string? Title { get; set; }
        public string? Color { get; set; }
        public string? EventType { get; set; }
        public string? EmployeeName { get; set; }
        public string? Reason { get; set; }
    }
}
