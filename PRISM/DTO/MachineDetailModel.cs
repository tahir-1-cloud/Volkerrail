using PRISM.Models;

namespace PRISM.DTO
{
	public class MachineDetailModel
	{
        public List<Machine> MachineList { get; set; }
		public List<LookupEntity> LookupList { get; set; }
	}
}
