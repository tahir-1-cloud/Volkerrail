namespace PRISM.DTO.ReportsModels
{
    public class InternalCommentReportModel
    {
        public long Id { get; set; }
        public string? StartDate { get; set; }
        public string? Shift { get; set; }
        public string? WeekNo { get; set; }
        public string? FinishDate { get; set; }
        public string? WorksiteDetails { get; set; }
        public string? StartTime { get; set; }
        public string? FinishTime { get; set; }
        public string? MachineNum { get; set; }
        public string? Customer { get; set; }
        public string? PpreDay { get; set; }
        public string? PPreInternalComments { get; set; }
    }
}
