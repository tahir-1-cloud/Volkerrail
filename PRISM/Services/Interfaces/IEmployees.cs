using PRISM.DTO.AppUsersModel;
using PRISM.DTO.Employeedto;
using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface IEmployees
	{
		Task<List<Employee>> GetData(int type,string SearchText="");
        Task<List<Role>> GetRoles();
        Task<Employee> GetDataById(int Id);
		Task<Employee> Insert(Employee param);
		Task<bool> delete(int Id);
        Task<List<ModuleRoleModels>> GetRolesAndRights(int RoleId);
        Task<List<ModuleRoleModels>> GetRolesAndRightsByUserId(string UserId);
        Task<string> InsertModuleRole(List<RoleModule> param);
        Task<string> InsertRole(Role param);
        Task<string> InsertAppUser(AppUser param);
        Task<string> UpdateRole(AppUser param,string UserId);
        Task<List<AppUserViewModel>> GetAppUsers(int RoleId);
        Task<List<UserLogModel>> GetUserLog(string UserId,int ShiftId);
        Task<List<UserLogModel>> GetUserLogForRole(string UserId, int UserIdFrom);

	}
}
