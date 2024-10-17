namespace PRISM.DTO.Employeedto
{
    public class ModuleRolesInputModel
    {
        public int ModuleId { get; set; }
        public int RoleId { get; set; }
         public bool? IsInsert { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsEdit { get; set; }
    }
 
}
