using PRISM.DTO.Employeedto;
using PRISM.Models;
using System.ComponentModel.DataAnnotations;

namespace PRISM.DTO.AbsencesFolder
{
    public class AbsenceModel
    {
        public List<Employee> EmployeeList { get; set; }
        public List<LookupEntity> LookupList { get; set; }
        public List<string> DepartmentList { get; set; }
        public List<LeaveType> LeaveTypeList { get; set; }
        public List<AbsenceDataModel> AbsanceList { get; set; }
        public int type { get; set; }
    }
    public class AbsenceDataModel
    {
        public long Id { get; set; }
        [Required]
        public DateTime? DateFrom { get; set; }
        [Required]
        public DateTime? DateTo { get; set; }
        [Required]
        public string? DateFromString { get; set; }
        [Required]
        public string? DateToString { get; set; }
        public int? LeaveTypeId { get; set; }
        public string? LeaveType { get; set; }
        public long? EmployeeId { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        public string? Reason { get; set; }
        public int RowIndex { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
