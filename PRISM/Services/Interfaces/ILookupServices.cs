using PRISM.DTO;
using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface ILookupServices
	{
		Task<List<LookupEntity>> GetLookups(List<string> type);
        Task<List<Week>> GetWeeks();
        Task<List<LeaveType>> GetLeaves();
        Task<List<ShiftTemplate>> GetTemplateData();
        Task<Week> Insert(WeeksModel param);
        Task<bool> deleteWeek(int Id);
        Task<LeaveType> Insert(LeaveType param);
        Task<bool> deleteLeave(int Id);
        Task<LookupEntity> Insert(LookupEntity param);
		Task<bool> delete(int Id);
        Task<List<ShiftTemplate>> GetTemplates();
        Task<bool> DeleteTemplate(int Id);
        Task<ShiftTemplate> InsertTemplate(ShiftTemplate param);
        Task<List<Role>> GetRoles();
    }
}
