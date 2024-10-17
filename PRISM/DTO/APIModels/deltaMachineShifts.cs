namespace PRISM.DTO.APIModels
{
    public class deltaMachineShifts
    {
        public int? machineShiftId { get; set; }
        public string? ptoNo { get; set; }
        public string? week { get; set; }
        public string? machineNumber { get; set; }
        public DateTime? start { get; set; }
        public string phiresV3Ref { get; set; }
        public string day { get; set; }
        public string machineType { get; set; }
        public string headCode { get; set; }
        public string customer { get; set; }
        public string route { get; set; }
        public string location { get; set; }
        public string stableFrom { get; set; }
        public string stableTo { get; set; }
        public string plannedWork { get; set; }
        public string activityRef { get; set; }

        public  string time { get; set; }
        public List<APIOperatorsModel> Operators { get; set; }

    }
    public class APIOperatorsModel
    {
        public long Id { get; set; }
        public bool IsLeader { get; set; }
        public string Name { get; set; }
    }
}
