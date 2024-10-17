using PRISM.Models;

namespace PRISM.DTO
{
	public class DistributionModel
    {
        public List<DistributionViewModel> DistributionList { get; set; }
		public List<LookupEntity> LookupList { get; set; }
	}
}
