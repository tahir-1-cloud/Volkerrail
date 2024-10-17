namespace PRISM.DTO.ChangeLogDTO
{
    public class ChangeLogModel
    {
        public long Id { get; set; }
        public string? Ptonumber { get; set; }
        public string? LogShiftDate { get; set; }
        public string? ChangedBy { get; set; }
        public string? ChangeDate { get; set; }
        public string? InstigatedBy { get; set; }
        public string? ContactName { get; set; }
        public string? ChangePeriod { get; set; }
        public string? ChangeType { get; set; }
        public string? MoreInformation { get; set; }
        public string? FurtherAction { get; set; }
        public long? ShiftId { get; set; }
        public string? MachineNum { get; set; }
    }
}
