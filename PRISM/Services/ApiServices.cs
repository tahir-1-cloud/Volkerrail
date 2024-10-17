
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PRISM.DTO.APIModels;
using PRISM.DTO.MainShift;
using PRISM.DTO.ReportsModels;
using PRISM.Models;
using System.Data;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace PRISM.Services
{
    public interface IApiServices
    {
        Task<List<deltaMachineShifts>> GetShifts(DateTime StartDate, DateTime? EndDate);
    }

    public class ApiServices : IApiServices
    {
        private readonly PRISMContext dBContext;
        private readonly IConfiguration configuration;
        public ApiServices(PRISMContext _dbContext, IConfiguration _configuration)
        {
            dBContext = _dbContext;
            configuration = _configuration;
        }

        public string start { get; private set; }

        public async Task<List<deltaMachineShifts>> GetShifts(DateTime StartDate, DateTime? EndDate)
        {

            RootReportModel resulReport = new RootReportModel();
            List<deltaMachineShifts> listReport = new List<deltaMachineShifts>(); ;

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetApiShifts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", StartDate);
                //cmd.Parameters.AddWithValue("@EndDate", EndDate);
                if (EndDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@EndDate", EndDate.Value);
                }
                else
                {
                    // If endDate is null, pass DBNull.Value to the parameter
                    cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                }
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    string ShiftNumber = Convert.ToInt32(rdr["ShiftNumber"]) < 10 ? Convert.ToString((0 + "" + Convert.ToInt32(rdr["ShiftNumber"]))) : Convert.ToString(rdr["ShiftNumber"]);


                    var employeeObject = await (from emp in dBContext.Employees
                                                join shiftemp in dBContext.ShiftDetailEmployees on emp.Id equals shiftemp.EmployeeId
                                                where shiftemp.ShiftId == Convert.ToInt32(rdr["machineShiftId"])
                                                //&& shiftemp.EmployeeType== "GridCrewManager"
                                                && (shiftemp.EmployeeType == "GridCrewManager" || shiftemp.EmployeeType == "GridCrewOperator")
                                                select new APIOperatorsModel
                                                {
                                                    Id = shiftemp.Id,
                                                    Name = emp.FirstName + " " + emp.LastName,
                                                    //IsLeader = true
                                                    IsLeader = (shiftemp.EmployeeType == "GridCrewManager" )? true : false                                              
                                                }).ToListAsync();

                   

                    var cvs = new deltaMachineShifts()
                    {

                        start = (rdr["StartDate"] != DBNull.Value && rdr["StartTime"] != DBNull.Value) ?
                                Convert.ToDateTime(rdr["StartDate"]).Date.Add(Convert.ToDateTime(rdr["StartTime"]).TimeOfDay) :
                                (DateTime?)null,
                        //start = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]),

                        stableFrom = Convert.ToString(rdr["OutShortCode"]) ?? "",                      
                        stableTo = Convert.ToString(rdr["InShortCode"]) ?? "",
                        machineShiftId = Convert.ToInt32(rdr["machineShiftId"]),
                        activityRef = Convert.ToString(rdr["activityRef"]) ?? "",
                        customer = Convert.ToString(rdr["Customer"]) ?? "",
                        day = Convert.ToString(rdr["PPreDay"]) ?? "",// crew manager
                        headCode = Convert.ToString(rdr["HeadCode"]) ?? "",
                        location = Convert.ToString(rdr["WorksiteDetails"]) ?? "",
                        machineNumber = Convert.ToString(rdr["MachineNum"]) ?? "",
                        machineType = Convert.ToString(rdr["MachineType"]) ?? "",
                        phiresV3Ref = ShiftNumber + "-" + ((Convert.ToString(rdr["WeekNo"])).Length > 2 ? Convert.ToString(rdr["WeekNo"]).Substring(2) : Convert.ToString(rdr["WeekNo"])) + "-" + Convert.ToString(rdr["WeekTitle"]).Substring(0, 4) + "-" + Convert.ToString(rdr["MachineNum"]),
                        plannedWork = Convert.ToString(rdr["PPrePlannedWork"]) ?? "",
                        ptoNo = Convert.ToString(rdr["PTONumber"]) ?? "",
                        route = Convert.ToString(rdr["Route"]) ?? "",
                        week = Convert.ToString(rdr["WeekNo"]) ?? "",
                        Operators = employeeObject
                    };
                    listReport.Add(cvs);
                }
                con.Close();
            }



            return listReport;

        }

    }


}
