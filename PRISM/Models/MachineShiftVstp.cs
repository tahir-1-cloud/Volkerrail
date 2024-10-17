using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class MachineShiftVstp
    {
        public long Id { get; set; }
        public int ShiftId { get; set; }
        public string? Vstpcontact { get; set; }
        public string? OriginLoc { get; set; }
        public string? OriginName { get; set; }
        public string? OriginStanox { get; set; }
        public string? OriginTime { get; set; }
        public string? DestLoc { get; set; }
        public string? DestName { get; set; }
        public string? DestStanox { get; set; }
        public string? DestTime { get; set; }
        public string? HeadCode { get; set; }
        public string? Comments { get; set; }
        public string? NumberOfVehicles { get; set; }
        public string? RecordStatus { get; set; }
        public string? ModifiedId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
