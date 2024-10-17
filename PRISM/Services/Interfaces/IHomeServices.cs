using PRISM.DTO.MainShift;
using PRISM.DTO.WeeklyCommentsdto;
using PRISM.Models;

namespace PRISM.Services.Interfaces
{
    public interface IHomeServices
    {
        Task<List<LNEDetailModel>> GetData(SearchShiftModel param,string UserId);
        Task<Dictionary<string, object>> GetDataById(int id);
        Task<LNEDetailModel> GetShiftDataById(int shiftId);

		Task<string> Insert(LNEDetailModel param, string UserId);
        Task<string> InsertGridMileStone(MileStoneEntry param, string UserId);
        Task<List<ShiftDetailEmployeeModel>> InsertPersons(ShiftDetailEmployeeModel param, string UserId);
        Task<string> CheckIfExist(ShiftDetailEmployeeModel param, string UserId);
        Task<string> InsertShiftContact(ShiftContact param, string UserId);
        Task<bool> delete(int Id);
        Task<List<MileStoneEntry>> GetLogTypes(string type, int shiftid,string UserId);
        Task<string> InsertBulkData(List<Lnedetail> param, string UserId);
        Task<bool> deleteGridMileStone(int Id, string UserId);
        Task<bool> deletePersons(int Id,string UserId);
        Task<bool> deleteShiftContact(int Id, string UserId);
        Task<DriverQuestion> GetDriverQuestion(int id, int driverId);
        Task<string> SaveDriverQuestion(DriverQuestion param);
        Task<string> InsertVSTP(MachineShiftVstp param);
        Task<string> SaveShiftStatus(string Status, List<int> Ids);
        Task<FilterHistory> GetFilterHistory(string UserId);
        Task<Lnedetail> DuplicateRecords(List<int> Ids,bool IsHoursAdded);
        Task<WeeklyCommentsModel> GetWeeklyComments(int WeekId);
        Task<string> SaveWeeklyComments(WeeklyCommentsModel param);
        Task<string> ResetFilters(string UserId);
        Task<string> GetLatestMUFNo();
        Task<Dictionary<string, object>> GetShiftDayWeek(string StartDateTime);
        Task<List<ShiftDetailContactTypeModel>> GetShiftDetailContact(int id);

	}
}
