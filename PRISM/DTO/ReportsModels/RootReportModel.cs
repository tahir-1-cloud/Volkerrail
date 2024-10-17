using PRISM.Models;

namespace PRISM.DTO.ReportsModels
{
    public class RootReportModel
    {
        public VSTPViewModel vSTPViewModel { get; set; }
        public List<MachineDepartmentsModel> boxReportModel { get; set; }
        public List<WeeklyRosterReportModel> weeklyRosterModel { get; set; }
        public WeeklyComment weeklyComment { get; set; }
        public List<WeekArrangement> weekArrangements { get; set; }
        public List<InternalCommentReportModel> internalCommentReport { get; set; }
        public List<ConductorChroneReportModel> conductorChroneReport { get; set; }
        public List<Week> WeekList { get; set; }
        public List<LookupEntity> LookupList { get; set; }
        public string MachineNumber { get; set; }
    }
}
