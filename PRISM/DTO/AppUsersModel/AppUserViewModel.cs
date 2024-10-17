using PRISM.DTO.Employeedto;
using PRISM.Models;

namespace PRISM.DTO.AppUsersModel
{
	public class AppUserViewModel
	{
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? EmailAddress { get; set; }
        public string? RecordStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedDateString { get; set; }
        public string? ModifiedDateString { get; set; }
        public int? RoleId { get; set; }
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
    }
}
