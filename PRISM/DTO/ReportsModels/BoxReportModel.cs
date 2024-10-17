namespace PRISM.DTO.ReportsModels
{
    public class BoxReportModel
    {
        public long Id { get; set; }
        public int RowNum { get; set; }
        public string? WeekNo { get; set; }
        public string? WorksiteDetails { get; set; }
        public string? StartTime { get; set; }
        public string? CalculatedTime { get; set; }
        public string? FinishTime { get; set; }
        public string? MachineNum { get; set; }
        public string? Conductor { get; set; }
        public string? PpreDriver { get; set; }
        public string? PpreDay { get; set; }
        public string? OutShortCode { get; set; }
        public string? InShortCode { get; set; }
        public string? WorkDescription { get; set; }
        public string? StartDateTime { get; set; }
        public string? FinishDateTime { get; set; }
        public string? OwnerName { get; set; }
        public string? WorksiteELR { get; set; }
        public string? PTONumber { get; set; }

        public string? shift {  get; set; }


    }
    public class MachineBoxReportModel
    {
        public string MachineNumber { get; set; }
        public bool IsShiftExist { get; set; }
        public List<BoxReportModel> boxReportModels { get; set; }
    }
    public class MachineDepartmentsModel
    {
        public string DepartmentName { get; set; }
        public List<MachineBoxReportModel> boxReportModel { get; set; }
    }
}
