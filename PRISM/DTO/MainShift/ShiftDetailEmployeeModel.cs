namespace PRISM.DTO.MainShift
{
    public class ShiftDetailEmployeeModel
    {
        public long ShiftId { get; set; }
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string JobTitle { get; set; }
        public string ContactNumber { get; set; }
        public string EmployeeType { get; set; }
        public string FullName { get; set; }
    }
}
