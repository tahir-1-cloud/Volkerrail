using PRISM.Models;

namespace PRISM.DTO
{
	public class TemplateViewModel
	{
        public List<ShiftTemplate> ShiftTemplateList { get; set; }
        public List<MainShiftColumnsModel> MainShiftColumns { get; set; }
    }

    public class MainShiftColumnsModel
    {
        public string ColumnName { get; set; }
        public bool ColumnValue { get; set; }
    }
}
