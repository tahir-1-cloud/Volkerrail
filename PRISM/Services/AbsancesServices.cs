using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PRISM.DTO.MainShift;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System.Data;
using PRISM.DTO.AbsencesFolder;
using Microsoft.Graph;
using System.Security.Claims;
using System.Globalization;
using System.Web.Http.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using PRISM.DTO;


namespace PRISM.Services
{
    public class Absanceservices : IAbsances
    {
        private readonly PRISMContext dBContext;
        private readonly IConfiguration configuration;
        public Absanceservices(PRISMContext _dbContext, IConfiguration _configuration)
        {
            dBContext = _dbContext;
            configuration = _configuration;
        }
        public async Task<List<AbsenceDataModel>> GetData(int PageIndex,int PageSize,string SortColumn,string SortOrder,string SearchText, int EmployeeId)
        {
            List<AbsenceDataModel> list = new List<AbsenceDataModel>();

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetAbsences", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.AddWithValue("@SortColumn", SortColumn);
                cmd.Parameters.AddWithValue("@SortOrder", SortOrder);
                cmd.Parameters.AddWithValue("@SearchText", SearchText);
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                con.Open();
                SqlDataReader rdr =await cmd.ExecuteReaderAsync();
                while (rdr.Read())
                {
                    var cvs = new AbsenceDataModel()
                    {
                        DateFromString = rdr["DateFromString"] == DBNull.Value ? null : Convert.ToString(rdr["DateFromString"]),
                        DateToString = rdr["DateToString"] == DBNull.Value ? null : Convert.ToString(rdr["DateToString"]),
                        EmployeeId = rdr["EmployeeId"] == DBNull.Value ? null : Convert.ToInt32(rdr["EmployeeId"]),
                        Id = Convert.ToInt32(rdr["Id"]),
                        RowIndex = Convert.ToInt32(rdr["RowIndex"]),
                        TotalRecords = Convert.ToInt32(rdr["TotalRecords"]),
                        TotalPages = Convert.ToInt32(rdr["TotalPages"]),
                        Start = Convert.ToInt32(rdr["Start"]),
                        End = Convert.ToInt32(rdr["End"]),
                        LeaveTypeId = rdr["LeaveTypeId"] == DBNull.Value ? null : Convert.ToInt32(rdr["LeaveTypeId"]),
                        LeaveType = rdr["LeaveType"] == DBNull.Value ? null : Convert.ToString(rdr["LeaveType"]),
                        Reason = rdr["Reason"] == DBNull.Value ? null : Convert.ToString(rdr["Reason"])
                    };

                    list.Add(cvs);
                }

                //while (rdr.Read())
                //{
                //    var cvs = new AbsenceDataModel()
                //    {
                //        DateFromString = Convert.ToString(rdr["DateFromString"]),
                //        DateToString = Convert.ToString(rdr["DateToString"]),
                //        EmployeeId = Convert.ToInt32(rdr["EmployeeId"]),
                //        Id = Convert.ToInt32(rdr["Id"]),
                //        RowIndex = Convert.ToInt32(rdr["RowIndex"]),
                //        TotalRecords = Convert.ToInt32(rdr["TotalRecords"]),
                //        TotalPages = Convert.ToInt32(rdr["TotalPages"]),
                //        Start = Convert.ToInt32(rdr["Start"]),
                //        End = Convert.ToInt32(rdr["End"]),
                //        LeaveTypeId = Convert.ToInt32(rdr["LeaveTypeId"]),
                //        LeaveType = Convert.ToString(rdr["LeaveType"]),
                //        Reason = Convert.ToString(rdr["Reason"])
                //    };

                //    list.Add(cvs);
                //}
                con.Close();
            }

            return list;

        }
        public async Task<Absance> Insert(AbsenceDataModel param,string UserId)
        {
            
            try
            {
                Absance obj = new Absance();
                if (!string.IsNullOrEmpty(param.DateFromString))
                {
                    if (DateTime.TryParseExact(param.DateFromString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        obj.DateFrom = result;
                        param.DateFrom = result;
                    }

                }
                if (!string.IsNullOrEmpty(param.DateToString))
                {
                    if (DateTime.TryParseExact(param.DateToString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        //obj.DateTo = result.AddHours(6);
                        //param.DateTo = result.AddHours(6);
                        obj.DateTo = result;
                        param.DateTo = result;
                    }

                }

                if (param.Id > 0)
                {
                     obj = await dBContext.Absances.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.DateFrom = param.DateFrom;
                        obj.DateTo = param.DateTo;
                        obj.LeaveTypeId = param.LeaveTypeId;
                        obj.EmployeeId = param.EmployeeId;
                        obj.ModifiedDate = DateTime.UtcNow;
                        obj.Reason = param.Reason;
                        obj.ModifiedBy = UserId;
                        obj.ModifiedDate = DateTime.UtcNow;
                        dBContext.Absances.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }

                    return obj;
                }
                else
                {
                    obj.CreatedDate = DateTime.UtcNow;
                    obj.ModifiedDate = DateTime.UtcNow;
                    obj.EmployeeId = param.EmployeeId;
                    obj.LeaveTypeId=param.LeaveTypeId;
                    obj.Reason=param.Reason;
                    obj.RecordStatus = "Active";
                    obj.CreatedBy = UserId;
                    dBContext.Absances.Add(obj);
                    await dBContext.SaveChangesAsync();

                    return obj;
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<bool> delete(int Id)
        {
            try
            {
                var obj = dBContext.Absances.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    obj.RecordStatus = "Deleted";
                    dBContext.Absances.Update(obj);
                    await dBContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
