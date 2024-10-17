namespace PRISM.DTO.ReportsModels
{
    public class WeeklyRosterReportModel
    {
        public long Id { get; set; }
        public string? WeekNo { get; set; }
        public DateTime? StartDate { get; set; }
        public string? WorksiteDetails { get; set; }
        public string? StartTime { get; set; }
        public DateTime? FinishDate { get; set; }
        public string? FinishTime { get; set; }
        public string? YardOut { get; set; }
        public string? YardIn { get; set; }
        public string? PathTime { get; set; }
        public string? MachineNum { get; set; }
        public string? Contractor { get; set; }
        public string? PpreOperator { get; set; }
        public string? PpreDriver { get; set; }
        public string? Customer { get; set; }
        public string? PpreDay { get; set; }
        public string? MachineMgr { get; set; }
        public string? PPreContractor { get; set; }
        public string? OutShortCode { get; set; }
        public string? InShortCode { get; set; }
        public string? PpreDra { get; set; }
        public string? STS { get; set; }
        public string? STSREM { get; set; }
        public string? StartDateTime { get; set; }
        public string? FinishDateTime { get; set; }
        public string? Color { get; set; }
    }
}
