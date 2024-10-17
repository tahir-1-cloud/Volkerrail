using PRISM.DTO.AbsencesFolder;
using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface IRoster
    {
        Task<List<RosterModel>> GetData(int empId, int FromWeek, int ToWeek);
        Task<string> SaveShiftRosterDescription(long ShiftId, string Description);
    }
}
