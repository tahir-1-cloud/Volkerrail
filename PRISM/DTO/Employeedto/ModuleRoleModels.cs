namespace PRISM.DTO.Employeedto
{
    public class ModuleRoleModels
    {
        public int ModuleId { get; set; }
        public int RoleId { get; set; }
        public string? ModuleName { get; set; }
		public string? ModuleIdentity { get; set; }
		public ActionRoleModels ActionRoles { get; set; }
    }
    public class ActionRoleModels
    {
        public bool? IsInsert { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsEdit { get; set; }
    }
}
