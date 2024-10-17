
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using NPOI.Util;
using PRISM.DTO.MainShift;
using PRISM.DTO.ReportsModels;
using PRISM.Models;
using System.Data;
using System.Linq;

namespace PRISM.Services
{
    public interface IReportServices
    {
        Task<RootReportModel> GetWeeklyRosterReport(int FromWeek, int ToWeek, string UserId);
        Task<RootReportModel> GetInternalCommentsReport(int FromWeek, int ToWeek, string UserId);
        Task<RootReportModel> GetConductionChronReport(int FromWeek, int ToWeek, string UserId);
        Task<RootReportModel> GetBoxReport(int FromWeek, int ToWeek, string UserId, string Dep);
    }

    public class ReportServices : IReportServices
    {
        private readonly PRISMContext dBContext;
        private readonly IConfiguration configuration;
        public ReportServices(PRISMContext _dbContext, IConfiguration _configuration)
        {
            dBContext = _dbContext;
            configuration = _configuration;
        }
        public async Task<RootReportModel> GetBoxReport(int FromWeek, int ToWeek, string UserId, string Dep)
        {

            RootReportModel resulReport = new RootReportModel();
            List<MachineDepartmentsModel> listMachineReport = new List<MachineDepartmentsModel>();
            var weekInfo = await dBContext.Weeks.Where(x => x.Id == FromWeek).FirstOrDefaultAsync();
            List<string> machineNumber = new List<string>();
            if (weekInfo != null)
            {
                try
                {
                    machineNumber = await dBContext.Lnedetails.Where(x => x.StartDate.Value.Date >= weekInfo.StartDate.Value.Date).Select(x => x.MachineNum).ToListAsync();
                }
                catch (Exception ex)
                {
                    machineNumber = new List<string>();
                }
            }
            var MachineList = new List<Machine>();
            if (!string.IsNullOrEmpty(Dep))
            {
                MachineList = await dBContext.Machines.Where(x => machineNumber.Contains(x.Number ?? "") && x.Number != "" && x.ManagerName == Dep).ToListAsync();
            }
            else
            {

                MachineList = await dBContext.Machines.Where(x => machineNumber.Contains(x.Number ?? "") && x.Number != "").OrderBy(x => x.ManagerName).ToListAsync();
            }
            MachineList = MachineList.Where(x => x.Number != "Cab Pass" && x.Number != "Video").ToList();
            // var machineNumber = await dBContext.Lnedetails.Where(x => x.StartDate).Select(x => x.Number).ToListAsync();
            if (MachineList?.Count > 0)
            {
                var departMentsList = MachineList.Select(x => x.ManagerName).Distinct().ToList();
                foreach (var department in departMentsList)
                {
                    MachineDepartmentsModel departmentBoxReportModel = new MachineDepartmentsModel();
                    departmentBoxReportModel.DepartmentName = department;
                    List<MachineBoxReportModel> listMachineModel = new List<MachineBoxReportModel>();
                    machineNumber = MachineList.Select(x => x.Number).Distinct().ToList();
                    foreach (var machine in machineNumber)
                    {
                        bool IsShiftExist = false;
                        MachineBoxReportModel machineBoxReportModel = new MachineBoxReportModel();
                        List<BoxReportModel> listReport = new List<BoxReportModel>();
                        using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
                        {
                            SqlCommand cmd = new SqlCommand("SpGetBoxReport", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@FromWeek", FromWeek);
                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@MachineNum", machine);
                            con.Open();
                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                            while (rdr.Read())
                            {

                                var cvs = new BoxReportModel()
                                {
                                    StartTime = Convert.ToString(rdr["StartTime"]),
                                    RowNum = Convert.ToInt32(rdr["RowNum"]),
                                    StartDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]).ToString("dd/MM/yyyy"),
                                    FinishDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]).ToString("dd/MM/yyyy"),
                                    Conductor = Convert.ToString(rdr["Conductor"]),
                                    FinishTime = Convert.ToString(rdr["FinishTime"]),
                                    Id = Convert.ToInt64(rdr["Id"]),
                                    MachineNum = Convert.ToString(rdr["MachineNum"]),
                                    PpreDay = Convert.ToString(rdr["PpreDay"]),
                                    WeekNo = Convert.ToString(rdr["WeekNo"]),
                                    WorksiteDetails = Convert.ToString(rdr["WorksiteDetails"]),
                                    PpreDriver = Convert.ToString(rdr["DriverName"]),
                                    WorkDescription = Convert.ToString(rdr["WorkDescription"]),
                                    OwnerName = Convert.ToString(rdr["OwnerName"]),
                                    InShortCode = Convert.ToString(rdr["InShortCode"]),
                                    OutShortCode = Convert.ToString(rdr["OutShortCode"]),
                                    PTONumber = Convert.ToString(rdr["PTONumber"]),
                                    CalculatedTime = Convert.ToString(rdr["CalculatedTime"]),
                                    WorksiteELR = Convert.ToString(rdr["ELR"]),
                                    shift = Convert.ToString(rdr["shift"]),
                                };

                                if (cvs.Id > 0)
                                {
                                    IsShiftExist = true;
                                }

                                listReport.Add(cvs);
                            }
                            con.Close();
                        }

                        machineBoxReportModel.MachineNumber = machine ?? "";
                        machineBoxReportModel.IsShiftExist = IsShiftExist;
                        machineBoxReportModel.boxReportModels = listReport;
                        listMachineModel.Add(machineBoxReportModel);

                    }
                    departmentBoxReportModel.boxReportModel = listMachineModel;
                    listMachineReport.Add(departmentBoxReportModel);
                }
            }

            resulReport.boxReportModel = listMachineReport;

            return resulReport;

        }
        public async Task<RootReportModel> GetWeeklyRosterReport(int FromWeek, int ToWeek, string UserId)
        {

            RootReportModel resulReport = new RootReportModel();
            List<WeeklyRosterReportModel> listReport = new List<WeeklyRosterReportModel>(); ;

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetWeeklyRosterReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromWeek", FromWeek);
                cmd.Parameters.AddWithValue("@ToWeek", ToWeek);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    string StartTime = Convert.ToDateTime(rdr["StartTime"]).ToString("HH:mm");
                    string FinishTime = Convert.ToDateTime(rdr["FinishTime"]).ToString("HH:mm");
                    //string StartTime = SecondsToTimeSpan(Convert.ToString(rdr["StartTime"]));
                    //string FinishTime = SecondsToTimeSpan(Convert.ToString(rdr["FinishTime"]));
                    var cvs = new WeeklyRosterReportModel()
                    {
                        StartTime = StartTime,
                        StartDate = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]),
                        FinishDate = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]),
                        StartDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]).ToString("dd/MM/yyyy") + StartTime,
                        FinishDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]).ToString("dd/MM/yyyy") + FinishTime,
                        Contractor = Convert.ToString(rdr["PPreContractor"]),
                        FinishTime = FinishTime,
                        Id = Convert.ToInt64(rdr["Id"]),
                        MachineNum = Convert.ToString(rdr["MachineNum"]),
                        PpreDay = Convert.ToString(rdr["PpreDay"]),
                        PpreOperator = Convert.ToString(rdr["PpreOperator"]),// crew manager
                        WeekNo = Convert.ToString(rdr["WeekNo"]),
                        WorksiteDetails = Convert.ToString(rdr["WorksiteDetails"]),
                        YardOut = Convert.ToString(rdr["YardOut"]),
                        YardIn = Convert.ToString(rdr["YardIn"]),
                        PpreDriver = Convert.ToString(rdr["PPreDriver"]),
                        STSREM = Convert.ToString(rdr["STSREM"]),
                        PathTime = Convert.ToString(rdr["PathTime"]),
                        STS = Convert.ToString(rdr["STS"]),
                        Color = Convert.ToString(rdr["STS"]) == "Caped" ? "red" : Convert.ToString(rdr["STS"]) == "PROV" ? "purple" : "black",
                        PpreDra = Convert.ToString(rdr["PpreDRA"])
                    };
                    listReport.Add(cvs);
                }
                con.Close();
            }


            resulReport.weeklyRosterModel = listReport;

            resulReport.weeklyComment = await dBContext.WeeklyComments.Where(x => x.WeekId == FromWeek).FirstOrDefaultAsync();
            resulReport.weekArrangements = await dBContext.WeekArrangements.Where(x => x.WeekId == FromWeek).ToListAsync();


            return resulReport;

        }
        public async Task<RootReportModel> GetInternalCommentsReport(int FromWeek, int ToWeek, string UserId)
        {

            RootReportModel resulReport = new RootReportModel();
            List<InternalCommentReportModel> listReport = new List<InternalCommentReportModel>(); ;

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetInternalComment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromWeek", FromWeek);
                cmd.Parameters.AddWithValue("@ToWeek", ToWeek);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    string StartTime = SecondsToTimeSpan(Convert.ToString(rdr["StartTime"]));
                    string FinishTime = SecondsToTimeSpan(Convert.ToString(rdr["FinishTime"]));
                    var cvs = new InternalCommentReportModel()
                    {
                        StartTime = StartTime,
                        StartDate = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]).ToString("dd/MM/yyyy") + StartTime,
                        FinishDate = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]).ToString("dd/MM/yyyy") + FinishTime,
                        FinishTime = FinishTime,
                        Id = Convert.ToInt64(rdr["Id"]),
                        MachineNum = Convert.ToString(rdr["MachineNum"]),
                        PpreDay = Convert.ToString(rdr["PpreDay"]),
                        Shift = Convert.ToString(rdr["Shift"]),// crew manager
                        WeekNo = Convert.ToString(rdr["WeekNo"]),
                        WorksiteDetails = Convert.ToString(rdr["WorksiteDetails"]),
                        Customer = Convert.ToString(rdr["Customer"]),
                        PPreInternalComments = Convert.ToString(rdr["PPreInternalComments"])
                    };
                    listReport.Add(cvs);
                }
                con.Close();
            }


            resulReport.internalCommentReport = listReport;

            return resulReport;

        }
        public async Task<RootReportModel> GetConductionChronReport(int FromWeek, int ToWeek, string UserId)
        {

            RootReportModel resulReport = new RootReportModel();
            List<ConductorChroneReportModel> listReport = new List<ConductorChroneReportModel>(); ;

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetConductorCronReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromWeek", FromWeek);
                cmd.Parameters.AddWithValue("@ToWeek", ToWeek);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    string StartTime = SecondsToTimeSpan(Convert.ToString(rdr["StartTime"]));
                    string FinishTime = SecondsToTimeSpan(Convert.ToString(rdr["FinishTime"]));
                    var cvs = new ConductorChroneReportModel()
                    {
                        StartTime = StartTime,
                        StartDateAndFinish = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]).ToString("dd/MM/yyyy") + StartTime,
                        Contractor = Convert.ToString(rdr["Contractor"]),
                        FinishTime = FinishTime,
                        Id = Convert.ToInt64(rdr["Id"]),
                        MachineNum = Convert.ToString(rdr["MachineNum"]),
                        PpreDay = Convert.ToString(rdr["PpreDay"]),
                        InShortCode = Convert.ToString(rdr["InShortCode"]),// crew manager
                        OutShortCode = Convert.ToString(rdr["OutShortCode"]),
                        WorksiteDetails = Convert.ToString(rdr["WorksiteDetails"]),
                        Customer = Convert.ToString(rdr["Customer"]),
                        MachineMgr = Convert.ToString(rdr["ManagerName"]),
                        PPrePlannedWork = Convert.ToString(rdr["PPrePlannedWork"]),
                        ReccyNo = Convert.ToString(rdr["ReccyNo"]),
                        Remarks = Convert.ToString(rdr["Remarks"]),
                        WorkDescription = Convert.ToString(rdr["WorkDescription"]),
                        WorksiteELR = Convert.ToString(rdr["WorksiteELR"])
                    };
                    listReport.Add(cvs);
                }
                con.Close();
            }


            resulReport.conductorChroneReport = listReport;


            return resulReport;

        }

        private string SecondsToTimeSpan(string seconds)
        {

            string timeWithoutSeconds = "";
            try
            {
                double totalSeconds = double.Parse(seconds);
                TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
                timeWithoutSeconds = timeSpan.ToString(@"hh\:mm\");

            }
            catch (Exception ex)
            {
                DateTime time = DateTime.ParseExact(seconds, "h:mm:ss tt", null);
                timeWithoutSeconds = time.ToString("hh:mm");
                //return timeWithoutSeconds;
            }
            finally
            {

            }

            return timeWithoutSeconds;
        }


    }


}
