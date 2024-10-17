using PRISM.DTO.MainShift;
using PRISM.Models;
using System.Reflection.PortableExecutable;

namespace PRISM.DTO.ReportsModels
{
    public class VSTPViewModel
    {
        public LNEDetailModel LNEDetailModel { get; set; }
        public Models.Machine MachineModel { get; set; }
        public MachineShiftVstp MachineShiftVstpModel { get; set; }
    }
}
