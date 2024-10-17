using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PRISM.DTO.MainShift;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System.Data;
using PRISM.DTO.AbsencesFolder;
using Nancy.Routing.Trie;

namespace PRISM.Services
{
    public class RosterServices : IRoster
    {
        private readonly PRISMContext dBContext;
        private readonly IConfiguration configuration;
        public RosterServices(PRISMContext _dbContext, IConfiguration _configuration)
        {
            dBContext = _dbContext;
            configuration = _configuration;
        }
        public async Task<List<RosterModel>> GetData(int empId,int FromWeek,int ToWeek)
        {
            List<RosterModel> list = new List<RosterModel>();

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("GetRosterEvents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", empId);
                cmd.Parameters.AddWithValue("@FromWeek", FromWeek);
                cmd.Parameters.AddWithValue("@ToWeek", ToWeek);
                con.Open();
                SqlDataReader rdr =await cmd.ExecuteReaderAsync();
                int counter = 1;
                while (rdr.Read())
                {
                    var cvs = new RosterModel()
                    {
                        StartDate = Convert.ToString(rdr["StartDate"]),
                        EndDate = Convert.ToString(rdr["EndDate"]),
                        EmployeeId = Convert.ToInt32(rdr["EmployeeId"]),
                        Id = counter,
                        LeaveTypeId = Convert.ToInt32(rdr["LeaveTypeId"]),
                        Title = Convert.ToString(rdr["Title"]),
                        Color = Convert.ToString(rdr["Color"]),
                        ShiftId = Convert.ToInt32(rdr["ShiftId"]),
                        EventType = Convert.ToString(rdr["EventType"]),
						EmployeeName = Convert.ToString(rdr["EmployeeName"]),
                        Reason = Convert.ToString(rdr["Reason"])
                    };

                    list.Add(cvs);

                    counter++;
                }
                con.Close();
            }

            return list;

        }
        public async Task<string> SaveShiftRosterDescription(long ShiftId,string Description)
        {
            try
            {
                using (var context = new PRISMContext())
                {
                    var obj = new ShiftRosterDetail()
                    {
                        ShiftId = ShiftId,
                        RosterShiftDescription = Description
                    };
                    context.ShiftRosterDetails.Add(obj);
                    await context.SaveChangesAsync();
                }
               

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
           
        }
    }
}
