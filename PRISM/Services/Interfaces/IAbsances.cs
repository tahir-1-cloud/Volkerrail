using PRISM.DTO.AbsencesFolder;
using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface IAbsances
    {
        Task<Absance> Insert(AbsenceDataModel param,string UserId);
        Task<bool> delete(int Id);
        Task<List<AbsenceDataModel>> GetData(int PageIndex,int PageSize,string SortColumn,string SortOrder,string SearchText,int EmployeeId);
    }
}
