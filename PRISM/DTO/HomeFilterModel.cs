using PRISM.DTO.AbsencesFolder;
using PRISM.Models;

namespace PRISM.DTO
{
    public class HomeFilterModel
    {
        public List<Machine> MachinesList { get; set; }
        public List<Week> WeekList { get; set; }
        public List<ShiftTemplate> TemplateList { get; set; }
        public List<LookupEntity> StatusList { get; set; }
        public EmployeeModel AbsanceModel { get; set; }
        public FilterHistory FilterHistory { get; set; }
        public List<Distribution> DistributionList { get; set; }
        public List<Models.Route> LocationList{ get; set; }
    }
}
