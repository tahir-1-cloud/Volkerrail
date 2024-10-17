namespace PRISM.DTO.ReportsModels
{
    public class CommentReportModel
    {
        public long? WeekId { get; set; }
        public string? EngineeringSupport { get; set; }
        public string? CoursesAndOthers { get; set; }
        public List<WeeklyArrangmentsModel> weeklyArrangmentsModels { get; set; }
    }

    public class WeeklyArrangmentsModel
    {
        public long? WeekId { get; set; }
        public string? ColumnNo1 { get; set; }
        public string? ColumnNo2 { get; set; }
        public string? ColumnNo3 { get; set; }
        public string? ColumnNo4 { get; set; }
        public string? ColumnNo5 { get; set; }
        public string? ColumnNo6 { get; set; }
        public string? ColumnNo7 { get; set; }
        public string? ColumnNo8 { get; set; }
    }
}
