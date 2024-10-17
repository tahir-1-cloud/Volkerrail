namespace PRISM.DTO.MainShift
{
    public class SearchShiftModel
    {
        public string Template { get; set; }
        public string FromWeek { get; set; }
        public string ToWeek { get; set; }
        public string LocationSearch { get; set; }
        public string MachineNumber { get; set; }
        public string MachineStatus { get; set; }
        public string MachineType { get; set; }
        public int Staff { get; set; }
        public string ShiftStatus { get; set; }
        public string Filters { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchText { get; set; }
        public string VRCCToday { get; set; }
        public bool OTM { get; set; }
        public bool OTPM { get; set; }
        public bool OTT { get; set; }
        public bool ACT { get; set; }
        public bool ShiftBlank { get; set; }
        public bool ShiftCancelled { get; set; }
        public bool ShiftCaped { get; set; }
        public bool Owner { get; set; }
    }
}
