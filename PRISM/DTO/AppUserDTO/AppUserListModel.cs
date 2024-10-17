using PRISM.DTO.AppUsersModel;
using PRISM.Models;

namespace PRISM.DTO.AppUserDTO
{
    public class AppUserListModel
    {
        public List<AppUserViewModel> appUsers { get; set; }
        public List<Role> userRoles { get; set; }
    }
}
