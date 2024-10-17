
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using NPOI.Util;
using PRISM.DTO;
using PRISM.DTO.AppUsersModel;
using PRISM.DTO.ChangeLogDTO;
using PRISM.DTO.MainShift;
using PRISM.DTO.ReportsModels;
using PRISM.Models;
using System.Data;
using System.Globalization;
using System.Text;

namespace PRISM.Services
{
    public interface IChangeLogServices
    {
        Task<string> Insert(ChangeLogModel param, string UserId);
        Task<List<ChangeLog>> GetChangeLog(string UserId, int ShiftId);
        Task<List<ChangeLog>> GetChangeLogByDate(int FromWeek, int ToWeek);
        Task<List<UserLog>> GetAuditLogByDate(string FromDate, string ToDate);
    }

    public class ChangeLogServices : IChangeLogServices
    {
        private readonly PRISMContext dBContext;
        private readonly IConfiguration configuration;
        public ChangeLogServices(PRISMContext _dbContext, IConfiguration _configuration)
        {
            dBContext = _dbContext;
            configuration = _configuration;
        }
        public async Task<string> Insert(ChangeLogModel param, string UserId)
        {
            try
            {
                ChangeLog obj = new ChangeLog();
                if (!string.IsNullOrEmpty(param.LogShiftDate))
                {
                    if (DateTime.TryParseExact(param.LogShiftDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        obj.LogShiftDate = result;

                    }

                }
                if (!string.IsNullOrEmpty(param.ChangeDate))
                {
                    if (DateTime.TryParseExact(param.ChangeDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        obj.ChangeDate = result;
                    }

                }


                obj.ShiftId = param.ShiftId;
                obj.Ptonumber = param.Ptonumber;
                obj.ChangePeriod = param.ChangePeriod;
                obj.ChangedBy = param.ChangedBy;
                obj.ChangeType = param.ChangeType;
                obj.ContactName = param.ContactName;
                obj.FurtherAction = param.FurtherAction;
                obj.InstigatedBy = param.InstigatedBy;
                obj.MoreInformation = param.MoreInformation;
                obj.MachineNum = param.MachineNum;
               
                
                dBContext.ChangeLogs.Add(obj);
                await dBContext.SaveChangesAsync();



                return "success";

            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

        public async Task<List<ChangeLog>> GetChangeLog(string UserId, int ShiftId)
        {

            var list=await dBContext.ChangeLogs.Where(x => x.ShiftId==ShiftId).ToListAsync();
            return list;
        }
        public async Task<List<UserLog>> GetAuditLogByDate(string FromDate, string ToDate)
        {
            DateTime FromDateTime = DateTime.Now;
            DateTime ToDateTime = DateTime.Now;
            if (!string.IsNullOrEmpty(FromDate))
            {
                if (DateTime.TryParseExact(FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    FromDateTime = result;

                }

            }
            if (!string.IsNullOrEmpty(ToDate))
            {
                if (DateTime.TryParseExact(ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    ToDateTime = result;
                }

            }


            var list = await dBContext.UserLogs.Where(x => x.CreatedDate.Value.Date >= FromDateTime.Date && x.CreatedDate.Value.Date <= ToDateTime.Date).ToListAsync();
            return list;
        }
        public async Task<List<ChangeLog>> GetChangeLogByDate(int FromWeek, int ToWeek)
        {
            DateTime FromDateTime = DateTime.Now;
            DateTime ToDateTime = DateTime.Now;
            //if (!string.IsNullOrEmpty(FromDate))
            //{
            //    if (DateTime.TryParseExact(FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            //    {
            //        FromDateTime = result;

            //    }

            //}
            //if (!string.IsNullOrEmpty(ToDate))
            //{
            //    if (DateTime.TryParseExact(ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            //    {
            //        ToDateTime = result;
            //    }

            //}
            var objWeekFrom=await dBContext.Weeks.Where(x => x.Id==FromWeek).FirstOrDefaultAsync();
            var objWeekTo = await dBContext.Weeks.Where(x => x.Id == ToWeek).FirstOrDefaultAsync();
            if (objWeekFrom != null && objWeekTo==null)
            {
                var objShiftIds = await dBContext.Lnedetails.Where(x => x.StartDate.Value.Date >= objWeekFrom.StartDate.Value.Date).Select(x => x.Id).ToListAsync();
                var list = await dBContext.ChangeLogs.Where(x => objShiftIds.Contains(x.ShiftId??0)).ToListAsync();
                return list;
            }
            else if (objWeekFrom != null && objWeekTo != null)
            {
                var objShiftIds = await dBContext.Lnedetails.Where(x => x.StartDate.Value.Date >= objWeekFrom.StartDate.Value.Date && x.StartDate.Value.Date <= objWeekTo.EndDate.Value.Date).Select(x => x.Id).ToListAsync();

                var list = await dBContext.ChangeLogs.Where(x => objShiftIds.Contains(x.ShiftId ?? 0)).ToListAsync();
                return list;
            }
            else
            {
                return null;
            }

        }

    }


}
