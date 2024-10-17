namespace PRISM.DTO.AbsencesFolder
{
    public class CommonInputModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchText { get; set; }
        public int EmployeeId { get; set; }
    }
}
