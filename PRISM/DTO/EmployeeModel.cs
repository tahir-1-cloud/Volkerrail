using PRISM.DTO.Employeedto;
using PRISM.Models;

namespace PRISM.DTO
{
	public class EmployeeModel
    {
		public List<Employee> EmployeeList { get; set; }
		public List<LookupEntity> LookupList { get; set; }
        public List<EmployeeTypeModel> EmployeeTypeCounts { get; set; }
		public List<string?> DepartmentList { get; set; }
        public List<LeaveType> LeaveTypeList { get; set; }
        public List<Absance> AbsanceList { get; set; }
        public int type { get; set; }
        public string EmpType { get; set; }
    }
}
