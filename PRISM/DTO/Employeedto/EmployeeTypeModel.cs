namespace PRISM.DTO.Employeedto
{
	public class EmployeeTypeModel
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public int Count { get; set; }
        public bool IsSelected { get; set; }
    }
}
