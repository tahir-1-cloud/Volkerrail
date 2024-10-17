using Microsoft.EntityFrameworkCore;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System.Data;
using PRISM.DTO.MainShift;
using Microsoft.Graph;
using Microsoft.Data.SqlClient;
using PRISM.DTO.AbsencesFolder;
using System.Drawing.Printing;
using System.Collections.Generic;
using AutoMapper;
using NPOI.SS.UserModel;
using PRISM.DTO.WeeklyCommentsdto;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using iText.Commons.Actions.Contexts;
using PRISM.DTO;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace PRISM.Services
{
    public class HomeServices : IHomeServices
    {
        private readonly PRISMContext dBContext;
        private readonly IConfiguration configuration;
        public HomeServices(PRISMContext _dbContext, IConfiguration _configuration)
        {
            dBContext = _dbContext;
            configuration = _configuration;
        }

        public async Task<List<LNEDetailModel>> GetData(SearchShiftModel param, string UserId)
        {
            List<LNEDetailModel> list = new List<LNEDetailModel>();

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("GetMainShiftData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageIndex", param.PageIndex);
                cmd.Parameters.AddWithValue("@PageSize", param.PageSize);
                cmd.Parameters.AddWithValue("@SortColumn", param.SortColumn);
                cmd.Parameters.AddWithValue("@SortOrder", param.SortOrder);
                cmd.Parameters.AddWithValue("@SearchText", param.SearchText);
                cmd.Parameters.AddWithValue("@Template", param.Template);
                cmd.Parameters.AddWithValue("@MachineNumber", param.MachineNumber);
                cmd.Parameters.AddWithValue("@MachineStatus", param.MachineStatus);
                cmd.Parameters.AddWithValue("@MachineType", param.MachineType);
                cmd.Parameters.AddWithValue("@Staff", param.Staff);
                cmd.Parameters.AddWithValue("@ShiftStatus", param.ShiftStatus);
                cmd.Parameters.AddWithValue("@FromWeek", string.IsNullOrEmpty(param.FromWeek) ? 0 : Convert.ToInt32(param.FromWeek));
                cmd.Parameters.AddWithValue("@ToWeek", string.IsNullOrEmpty(param.ToWeek) ? 0 : Convert.ToInt32(param.ToWeek));
                cmd.Parameters.AddWithValue("@Filters", param.VRCCToday == "VRCCToday" ? DateTime.Now.AddDays(-12) : null);
                cmd.Parameters.AddWithValue("@OTM", param.OTM);
                cmd.Parameters.AddWithValue("@OTPM", param.OTPM);
                cmd.Parameters.AddWithValue("@OTT", param.OTT);
                cmd.Parameters.AddWithValue("@ACT", param.ACT);
                cmd.Parameters.AddWithValue("@ShiftBlank", param.ShiftBlank);
                cmd.Parameters.AddWithValue("@ShiftCaped", param.ShiftCaped);
                cmd.Parameters.AddWithValue("@ShiftCancelled", param.ShiftCancelled);
                cmd.Parameters.AddWithValue("@Owner", param.Owner);
                cmd.Parameters.AddWithValue("@LocationSearch", param.LocationSearch);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    string StartTime = SecondsToTimeSpan(Convert.ToString(rdr["StartTime"]));
                    string FinishTime = SecondsToTimeSpan(Convert.ToString(rdr["FinishTime"]));
                    var cvs = new LNEDetailModel()
                    {
                        Shift = Convert.ToString(rdr["Shift"]),
                        StartTime = StartTime,
                        Supplier = Convert.ToString(rdr["Supplier"]),
                        ConsistSleeperType = Convert.ToString(rdr["ConsistSleeperType"]),
                        MachineSupply = Convert.ToString(rdr["MachineSupply"]),
                        OrderStatus = Convert.ToString(rdr["OrderStatus"]),
                        StartDate = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]),
                        FinishDate = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]),
                        PpreShiftDate = string.IsNullOrEmpty(Convert.ToString(rdr["PpreShiftDate"])) ? null : Convert.ToDateTime(rdr["PpreShiftDate"]),
                        StartDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]).ToString("MM-dd-yyyy") + " " + StartTime,
                        FinishDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]).ToString("MM-dd-yyyy") + " " + FinishTime,
                        Start = Convert.ToInt32(rdr["Start"]),
                        PpreSummary = Convert.ToString(rdr["PpreSummary"]),
                        PriorityStatusShift = Convert.ToString(rdr["PriorityStatusShift"]),
                        CancelledBy = Convert.ToString(rdr["CancelledBy"]),
                        CnupledMachines = Convert.ToString(rdr["CnupledMachines"]),
                        Contractor = Convert.ToString(rdr["Contractor"]),
                        Customer = Convert.ToString(rdr["Customer"]),
                        End = Convert.ToInt32(rdr["End"]),
                        EnteredBy = Convert.ToString(rdr["EnteredBy"]),
                        FinishTime = FinishTime,
                        Id = Convert.ToInt64(rdr["Id"]),
                        JobNumber = Convert.ToString(rdr["JobNumber"]),
                        LineToArriveOn = Convert.ToString(rdr["LineToArriveOn"]),
                        MachineType = Convert.ToString(rdr["MachineType"]),
                        MachineNum = Convert.ToString(rdr["MachineNum"]),
                        //OrderDate = Convert.ToString(rdr["OrderDate"]),
                        //Mileage = Convert.ToString(rdr["Mileage"]),
                        OrderRevision = Convert.ToString(rdr["OrderRevision"]),
                        PartOfBlockade = Convert.ToString(rdr["PartOfBlockade"]),
                        PossessionDetails = Convert.ToString(rdr["PossessionDetails"]),
                        PpreAssessor = Convert.ToString(rdr["PpreAssessor"]),
                        PpreCompany = Convert.ToString(rdr["PpreCompany"]),
                        PpreDay = Convert.ToString(rdr["PpreDay"]),
                        PpreDriver = Convert.ToString(rdr["PpreDriver"]),
                        PpreEmptyComment = Convert.ToString(rdr["PpreEmptyComment"]),
                        PpreFailureComments = Convert.ToString(rdr["PpreFailureComments"]),
                        PpreHeadCode = Convert.ToString(rdr["PpreHeadCode"]),
                        PpreInternalComments = Convert.ToString(rdr["PpreInternalComments"]),
                        PpreLogNumber = Convert.ToString(rdr["PpreLogNumber"]),
                        PpreOperator = Convert.ToString(rdr["PpreOperator"]),
                        PprePathTime = Convert.ToString(rdr["PprePathTime"]),
                        PprePlannedHours = Convert.ToString(rdr["PprePlannedHours"]),
                        PprePlannedWork = Convert.ToString(rdr["PprePlannedWork"]),
                        WonNumber = Convert.ToString(rdr["WonNumber"]),
                        Ppsreference = Convert.ToString(rdr["Ppsreference"]),
                        Ptonumber = Convert.ToString(rdr["Ptonumber"]),
                        Route = Convert.ToString(rdr["Route"]),
                        Remarks = Convert.ToString(rdr["Remarks"]),
                        RowIndex = Convert.ToInt32(rdr["RowIndex"]),
                        TimeOutOfYard = Convert.ToString(rdr["TimeOutOfYard"]),
                        TotalRecords = Convert.ToInt32(rdr["TotalRecords"]),
                        TotalPages = Convert.ToInt32(rdr["TotalPages"]),
                        TrainId = Convert.ToString(rdr["TrainId"]),
                        TrainOrderType = Convert.ToString(rdr["TrainOrderType"]),
                        WorkDescription = Convert.ToString(rdr["WorkDescription"]),
                        WorksiteDescriptor = Convert.ToString(rdr["WorksiteDescriptor"]),
                        WeekNo = Convert.ToString(rdr["WeekNo"]),
                        WorksiteDetails = Convert.ToString(rdr["WorksiteDetails"]),
                        WorksiteElr = Convert.ToString(rdr["WorksiteElr"]),
                        WorksiteNo = Convert.ToString(rdr["WorksiteNo"]),
                        WorkYardage = Convert.ToString(rdr["WorkYardage"]),
                        YardIn = Convert.ToString(rdr["YardIn"]),
                        YardOut = Convert.ToString(rdr["YardOut"]),
                        TimeBackToYard = Convert.ToString(rdr["TimeBackToYard"]),
                        CancellationDate = Convert.ToString(rdr["CancellationDate"]),
                        ShiftNo = Convert.ToString(rdr["ShiftNo"]),
                        MachineMgr = Convert.ToString(rdr["MachineMgr"]),
                        PpreTQSPhone = Convert.ToString(rdr["PPreTQSPhone"]),
                        PprePICOPPhone = Convert.ToString(rdr["PPrePICOPPhone"]),
                        HeadCode = Convert.ToString(rdr["HeadCode"]),
                        OutShortCode = Convert.ToString(rdr["OutShortCode"]),
                        InShortCode = Convert.ToString(rdr["InShortCode"]),
                        OutName = Convert.ToString(rdr["OutName"]),
                        InName = Convert.ToString(rdr["InName"]),
                        OutStanox = Convert.ToString(rdr["OutStanox"]),
                        InStanox = Convert.ToString(rdr["InStanox"]),
                        PpreAct = Convert.ToString(rdr["PpreACT"]),
                        PpreDra = Convert.ToString(rdr["PpreDRA"]),
                        PprePlfield = Convert.ToString(rdr["PPrePLField"]),
                        ShiftTimeDetail = Convert.ToString(rdr["ShiftTimeDetail"]),
                        WorkCompleted = Convert.ToString(rdr["WorkCompleted"]),
                        PpreAPM = "",
                        PpreAPMPhone = "",
                        PpreTQS = "",
                        PpreES = "",
                        PpreMCO = "",
                        PpreRTNo = "",
                        PpreOTML = Convert.ToString(rdr["PpreOTML"]),
                        PPreContractor = Convert.ToString(rdr["PPreContractor"])
                    };

                    list.Add(cvs);
                }
                con.Close();
            }


            if (!string.IsNullOrEmpty(UserId))
            {
                var obj = dBContext.FilterHistories.Where(x => x.UserId == UserId).FirstOrDefault();
                if (obj != null)
                {
                    obj.FromWeek = param.FromWeek;
                    obj.Template = param.Template;
                    obj.ToWeek = param.ToWeek;
                    obj.Otm = param.OTM;
                    obj.Otpm = param.OTPM;
                    obj.Ott = param.OTT;
                    obj.Act = param.ACT;
                    obj.ShiftBlank = param.ShiftBlank;
                    obj.ShiftCaped = param.ShiftCaped;
                    obj.ShiftCancelled = param.ShiftCancelled;
                    obj.Owner = param.Owner;
                    // obj.ShiftStatus = param.ShiftStatus;
                    // obj.MachineStatus = param.MachineStatus;
                    obj.MachineNumber = param.MachineNumber;
                    dBContext.FilterHistories.Update(obj);
                    await dBContext.SaveChangesAsync();
                }
                else
                {
                    obj = new FilterHistory()
                    {
                        FromWeek = param.FromWeek,
                        // ToWeek = param.ToWeek,
                        Otm = param.OTM,
                        // ShiftStatus = param.ShiftStatus,
                        // MachineStatus = param.MachineStatus,
                        MachineNumber = param.MachineNumber,
                        UserId = UserId,
                        ShiftCaped = param.ShiftCaped,
                        ShiftCancelled = param.ShiftCancelled,
                        ShiftBlank = param.ShiftBlank,
                        Act = param.ACT,
                        Otpm = param.OTPM,
                        Owner = param.Owner,
                        Template = param.Template
                    };
                    dBContext.FilterHistories.Add(obj);
                    await dBContext.SaveChangesAsync();
                }
            }

            return list;

        }

        public async Task<Dictionary<string, object>> GetDataById(int id)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            try
            {
                var employeeObject = await (from emp in dBContext.Employees
                                            join shiftemp in dBContext.ShiftDetailEmployees on emp.Id equals shiftemp.EmployeeId
                                            where shiftemp.ShiftId == id
                                            select new ShiftDetailEmployeeModel
                                            {
                                                EmployeeId = shiftemp.EmployeeId ?? 0,
                                                ContactNumber = shiftemp.ContactNumber ?? "",
                                                Id = shiftemp.Id,
                                                JobTitle = shiftemp.JobTitle ?? "",
                                                EmployeeType = shiftemp.EmployeeType ?? "",
                                                FullName = emp.FirstName + " " + emp.LastName
                                            }).ToListAsync();

                //var employeeObject = await dBContext.ShiftDetailEmployees.Where(x => x.ShiftId == id).ToListAsync();
                //var milestoneObject = await dBContext.MileStoneEntries.Where(x => x.ShiftId == id && x.RecordStatus != "Deleted").OrderBy(item => item.LogEntry != "TopLine").ThenBy(item => item.LogEntry).ToListAsync();


                var milestoneObject = await dBContext.MileStoneEntries.Where(x => x.ShiftId == id && x.RecordStatus != "Deleted" && x.LogEntry != "TopLine").ToListAsync();
                var topline = await dBContext.MileStoneEntries.Where(x => x.ShiftId == id && x.RecordStatus == "Active" && x.LogEntry == "TopLine").FirstOrDefaultAsync();
                if (topline != null)
                {
                    milestoneObject.Insert(0, topline);
                    // milestoneObject.Prepend(topline);
                }

                var contactObject = await (from cont in dBContext.Contacts
                                           join shiftcon in dBContext.ShiftContacts on cont.Id equals shiftcon.ContactId
                                           where shiftcon.ShiftId == id
                                           select new ShiftDetailContactTypeModel
                                           {
                                               Company = cont.Company ?? "",
                                               ContactId = shiftcon.ContactId,
                                               Id = shiftcon.Id,
                                               ContactType = shiftcon.ContactType ?? "",
                                               PhoneNumber = cont.MobileNumber
                                           }).ToListAsync();

                var RoutesM = await dBContext.Routes.Where(x => x.RecordStatus != "Deleted").ToListAsync();

                response.Add("EmployeeObject", employeeObject);
                response.Add("MilesStoneObject", milestoneObject);
                response.Add("ContactTypeObject", contactObject);
                response.Add("RoutesList", RoutesM);

            }
            catch (Exception ex)
            {
                response.Add("Error", ex.Message);

            }
            return response;
        }

        public async Task<List<ShiftDetailContactTypeModel>> GetShiftDetailContact(int id)
        {
            var contactObject = await (from cont in dBContext.Contacts
                                       join shiftcon in dBContext.ShiftContacts on cont.Id equals shiftcon.ContactId
                                       where shiftcon.ShiftId == id
                                       select new ShiftDetailContactTypeModel
                                       {
                                           Company = cont.Company ?? "",
                                           ContactId = shiftcon.ContactId,
                                           Id = shiftcon.Id,
                                           ContactType = shiftcon.ContactType ?? "",
                                           PhoneNumber = cont.MobileNumber
                                       }).ToListAsync();

            return contactObject;
        }
        public async Task<LNEDetailModel> GetShiftDataById(int shiftId)
        {
            LNEDetailModel cvs = new LNEDetailModel();
            try
            {
                using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
                {
                    SqlCommand cmd = new SqlCommand("GetMainShiftDataById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ShiftId", shiftId);
                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        string StartTime = SecondsToTimeSpan(Convert.ToString(rdr["StartTime"]));
                        string FinishTime = SecondsToTimeSpan(Convert.ToString(rdr["FinishTime"]));
                        cvs = new LNEDetailModel()
                        {
                            Shift = Convert.ToString(rdr["Shift"]),
                            StartTime = StartTime,
                            Supplier = Convert.ToString(rdr["Supplier"]),
                            ConsistSleeperType = Convert.ToString(rdr["ConsistSleeperType"]),
                            MachineSupply = Convert.ToString(rdr["MachineSupply"]),
                            OrderStatus = Convert.ToString(rdr["OrderStatus"]),
                            StartDate = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]),
                            FinishDate = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]),
                            PpreShiftDate = string.IsNullOrEmpty(Convert.ToString(rdr["PpreShiftDate"])) ? null : Convert.ToDateTime(rdr["PpreShiftDate"]),
                            StartDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["StartDate"])) ? null : Convert.ToDateTime(rdr["StartDate"]).ToString("MM-dd-yyyy") + " " + StartTime,
                            FinishDateTime = string.IsNullOrEmpty(Convert.ToString(rdr["FinishDate"])) ? null : Convert.ToDateTime(rdr["FinishDate"]).ToString("MM-dd-yyyy") + " " + FinishTime,
                            PpreSummary = Convert.ToString(rdr["PpreSummary"]),
                            PriorityStatusShift = Convert.ToString(rdr["PriorityStatusShift"]),
                            CancelledBy = Convert.ToString(rdr["CancelledBy"]),
                            CnupledMachines = Convert.ToString(rdr["CnupledMachines"]),
                            Contractor = Convert.ToString(rdr["Contractor"]),
                            Customer = Convert.ToString(rdr["Customer"]),
                            EnteredBy = Convert.ToString(rdr["EnteredBy"]),
                            FinishTime = FinishTime,
                            Id = Convert.ToInt64(rdr["Id"]),
                            JobNumber = Convert.ToString(rdr["JobNumber"]),
                            LineToArriveOn = Convert.ToString(rdr["LineToArriveOn"]),
                            MachineType = Convert.ToString(rdr["MachineType"]),
                            MachineNum = Convert.ToString(rdr["MachineNum"]),
                            //OrderDate = Convert.ToString(rdr["OrderDate"]),
                            //Mileage = Convert.ToString(rdr["Mileage"]),
                            OrderRevision = Convert.ToString(rdr["OrderRevision"]),
                            PartOfBlockade = Convert.ToString(rdr["PartOfBlockade"]),
                            PossessionDetails = Convert.ToString(rdr["PossessionDetails"]),
                            PpreAssessor = Convert.ToString(rdr["PpreAssessor"]),
                            PpreCompany = Convert.ToString(rdr["PpreCompany"]),
                            PpreDay = Convert.ToString(rdr["PpreDay"]),
                            PpreDriver = Convert.ToString(rdr["PpreDriver"]),
                            PpreEmptyComment = Convert.ToString(rdr["PpreEmptyComment"]),
                            PpreFailureComments = Convert.ToString(rdr["PpreFailureComments"]),
                            PpreHeadCode = Convert.ToString(rdr["PpreHeadCode"]),
                            PpreInternalComments = Convert.ToString(rdr["PpreInternalComments"]),
                            PpreLogNumber = Convert.ToString(rdr["PpreLogNumber"]),
                            PpreOperator = Convert.ToString(rdr["PpreOperator"]),
                            PprePathTime = Convert.ToString(rdr["PprePathTime"]),
                            PprePlannedHours = Convert.ToString(rdr["PprePlannedHours"]),
                            PprePlannedWork = Convert.ToString(rdr["PprePlannedWork"]),
                            WonNumber = Convert.ToString(rdr["WonNumber"]),
                            Ppsreference = Convert.ToString(rdr["Ppsreference"]),
                            Ptonumber = Convert.ToString(rdr["Ptonumber"]),
                            Route = Convert.ToString(rdr["Route"]),
                            Remarks = Convert.ToString(rdr["Remarks"]),
                            TimeOutOfYard = Convert.ToString(rdr["TimeOutOfYard"]),
                            TrainId = Convert.ToString(rdr["TrainId"]),
                            TrainOrderType = Convert.ToString(rdr["TrainOrderType"]),
                            WorkDescription = Convert.ToString(rdr["WorkDescription"]),
                            WorksiteDescriptor = Convert.ToString(rdr["WorksiteDescriptor"]),
                            WeekNo = Convert.ToString(rdr["WeekNo"]),
                            WorksiteDetails = Convert.ToString(rdr["WorksiteDetails"]),
                            WorksiteElr = Convert.ToString(rdr["WorksiteElr"]),
                            WorksiteNo = Convert.ToString(rdr["WorksiteNo"]),
                            WorkYardage = Convert.ToString(rdr["WorkYardage"]),
                            YardIn = Convert.ToString(rdr["YardIn"]),
                            YardOut = Convert.ToString(rdr["YardOut"]),
                            TimeBackToYard = Convert.ToString(rdr["TimeBackToYard"]),
                            CancellationDate = Convert.ToString(rdr["CancellationDate"]),
                            ShiftNo = Convert.ToString(rdr["ShiftNo"]),
                            MachineMgr = Convert.ToString(rdr["MachineMgr"]),
                            PpreTQSPhone = Convert.ToString(rdr["PPreTQSPhone"]),
                            PprePICOPPhone = Convert.ToString(rdr["PPrePICOPPhone"]),
                            HeadCode = Convert.ToString(rdr["HeadCode"]),
                            OutShortCode = Convert.ToString(rdr["OutShortCode"]),
                            InShortCode = Convert.ToString(rdr["InShortCode"]),
                            OutName = Convert.ToString(rdr["OutName"]),
                            InName = Convert.ToString(rdr["InName"]),
                            OutStanox = Convert.ToString(rdr["OutStanox"]),
                            InStanox = Convert.ToString(rdr["InStanox"]),
                            PpreAct = Convert.ToString(rdr["PpreACT"]),
                            PpreDra = Convert.ToString(rdr["PpreDRA"]),
                            PprePlfield = Convert.ToString(rdr["PPrePLField"]),
                            ShiftTimeDetail = Convert.ToString(rdr["ShiftTimeDetail"]),
                            PpreAPM = "",
                            PpreAPMPhone = "",
                            PpreTQS = "",
                            PpreES = "",
                            PpreMCO = "",
                            PpreRTNo = "",
                            PpreOTML = "",
                            PPreContractor = Convert.ToString(rdr["PPreContractor"])
                        };

                    }
                    con.Close();
                }

                return cvs;
            }
            catch (Exception ex)
            {
                return cvs;
            }




        }
        private string SecondsToTimeSpan(string seconds)
        {

            string timeWithoutSeconds = "";
            try
            {
                DateTime time = DateTime.ParseExact(seconds, "h:mm:ss tt", null);
                timeWithoutSeconds = time.ToString("HH:mm");

            }
            catch (Exception ex)
            {
                double totalSeconds = double.Parse(seconds);
                TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
                timeWithoutSeconds = timeSpan.ToString(@"HH\:mm\");

                //return timeWithoutSeconds;
            }
            finally
            {

            }

            return timeWithoutSeconds;
        }
        public async Task<string> Insert(LNEDetailModel param, string UserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(param.StartDateTime))
                {
                    if (DateTime.TryParseExact(param.StartDateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        param.StartDate = result;
                        if (string.IsNullOrEmpty(param.FinishDateTime))
                        {
                            param.FinishDate = result;
                        }
                    }

                }
                if (!string.IsNullOrEmpty(param.FinishDateTime))
                {
                    if (DateTime.TryParseExact(param.FinishDateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        param.FinishDate = result;
                    }

                }

                if (param.StartDate > param.FinishDate)
                {
                    return "Start Date is greater than Finish Date.";
                }

                var obj = await dBContext.Lnedetails.FindAsync(param.Id);

                if (obj != null)
                {

                    param.FinishTime = param.FinishTime + ":00";
                    param.StartTime = param.StartTime + ":00";
                    if (DateTime.TryParseExact(param.FinishTime, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                    {
                        param.FinishTime = dateTime.ToString("h:mm:ss tt");
                    }
                    else
                    {
                        param.FinishTime = "00:00:00 PM";
                    }

                    if (DateTime.TryParseExact(param.StartTime, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime2))
                    {
                        param.StartTime = dateTime2.ToString("h:mm:ss tt");
                    }
                    else
                    {
                        param.StartTime = "00:00:00 PM";
                    }
                    var NewObject = AssignOldData(param);
                    // List<ModifiedPropertiesModel> modifiedProperties = GetModifiedProperties(obj, NewObject);

                    StringBuilder stringBuilder = new StringBuilder();
                    // if (MatchStringValue(obj.PprePlannedHours, NewObject.PprePlannedHours))
                    //    stringBuilder.AppendLine($"Property Planned Hours was modified. Old value: '{obj.PprePlannedHours}', New value: '{NewObject.PprePlannedHours}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.CnupledMachines, NewObject.CnupledMachines))
                        stringBuilder.AppendLine($" CnupledMachines  was modified. Old value: '{obj.CnupledMachines}', New value: '{NewObject.CnupledMachines}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PprePlannedWork, NewObject.PprePlannedWork))
                        stringBuilder.AppendLine($" Planned Work was modified. Old value: '{obj.PprePlannedWork}', New value: '{NewObject.PprePlannedWork}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreSummary, NewObject.PpreSummary))
                        stringBuilder.AppendLine($" Summary was modified. Old value: '{obj.PpreSummary}', New value: '{NewObject.PpreSummary}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.WonNumber, NewObject.WonNumber))
                        stringBuilder.AppendLine($" Won Number was modified. Old value: '{obj.WonNumber}', New value: '{NewObject.WonNumber}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreAssessor, NewObject.PpreAssessor))
                        stringBuilder.AppendLine($" Assessor was modified. Old value: '{obj.PpreAssessor}', New value: '{NewObject.PpreAssessor}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.Ppsreference, NewObject.Ppsreference))
                        stringBuilder.AppendLine($"Worksite Refernce was modified. Old value: '{obj.Ppsreference}', New value: '{NewObject.Ppsreference}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreDay, NewObject.PpreDay))
                        stringBuilder.AppendLine($" Day was modified. Old value: '{obj.PpreDay}', New value: '{NewObject.PpreDay}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.Route, NewObject.Route))
                        stringBuilder.AppendLine($"Route was modified. Old value: '{obj.Route}', New value: '{NewObject.Route}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.FinishTime, NewObject.FinishTime))
                        stringBuilder.AppendLine($"Finish Time was modified. Old value: '{obj.FinishTime}', New value: '{NewObject.FinishTime}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.StartTime, NewObject.StartTime))
                        stringBuilder.AppendLine($" Start Time was modified. Old value: '{obj.StartTime}', New value: '{NewObject.StartTime}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreShiftDate, NewObject.PpreShiftDate))
                        stringBuilder.AppendLine($" Shift Date was modified. Old value: '{obj.PpreShiftDate}', New value: '{NewObject.PpreShiftDate}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.MachineNum, NewObject.MachineNum))
                        stringBuilder.AppendLine($" Machine Number was modified. Old value: '{obj.MachineNum}', New value: '{NewObject.MachineNum}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreCompany, NewObject.PpreCompany))
                        stringBuilder.AppendLine($" Company was modified. Old value: '{obj.PpreCompany}', New value: '{NewObject.PpreCompany}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreEmptyComment, NewObject.PpreEmptyComment))
                        stringBuilder.AppendLine($" Comment was modified. Old value: '{obj.PpreEmptyComment}', New value: '{NewObject.PpreEmptyComment}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreFailureComments, NewObject.PpreFailureComments))
                        stringBuilder.AppendLine($" Failure Comments was modified. Old value: '{obj.PpreFailureComments}', New value: '{NewObject.PpreFailureComments}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreHeadCode, NewObject.PpreHeadCode))
                        stringBuilder.AppendLine($" Head Code was modified. Old value: '{obj.PpreHeadCode}', New value: '{NewObject.PpreHeadCode}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PprePathTime, NewObject.PprePathTime))
                        stringBuilder.AppendLine($" Path Time was modified. Old value: '{obj.PprePathTime}', New value: '{NewObject.PprePathTime}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.Shift, NewObject.Shift))
                        stringBuilder.AppendLine($" Shift Status was modified. Old value: '{obj.Shift}', New value: '{NewObject.Shift}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreAct, NewObject.PpreAct))
                        stringBuilder.AppendLine($" ACT was modified. Old value: '{obj.PpreAct}', New value: '{NewObject.PpreAct}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreLogNumber, NewObject.PpreLogNumber))
                        stringBuilder.AppendLine($" Log Number was modified. Old value: '{obj.PpreLogNumber}', New value: '{NewObject.PpreLogNumber}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreInternalComments, NewObject.PpreInternalComments))
                        stringBuilder.AppendLine($" Internal Comments was modified. Old value: '{obj.PpreInternalComments}', New value: '{NewObject.PpreInternalComments}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.StartDate, NewObject.StartDate))
                        stringBuilder.AppendLine($" Start Date was modified. Old value: '{obj.StartDate}', New value: '{NewObject.StartDate}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.FinishDate, NewObject.FinishDate))
                        stringBuilder.AppendLine($" Finish Date was modified. Old value: '{obj.FinishDate}', New value: '{NewObject.FinishDate}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PpreDra, NewObject.PpreDra))
                        stringBuilder.AppendLine($" DRA was modified. Old value: '{obj.PpreDra}', New value: '{NewObject.PpreDra}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.Ptonumber, NewObject.Ptonumber))
                        stringBuilder.AppendLine($" PTO Number was modified. Old value: '{obj.Ptonumber}', New value: '{NewObject.Ptonumber}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.WorkDescription, NewObject.WorkDescription))
                        stringBuilder.AppendLine($" Work Description was modified. Old value: '{obj.WorkDescription}', New value: '{NewObject.WorkDescription}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.WorksiteDescriptor, NewObject.WorksiteDescriptor))
                        stringBuilder.AppendLine($" Worksite Descriptor was modified. Old value: '{obj.WorksiteDescriptor}', New value: '{NewObject.WorksiteDescriptor}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.WorksiteDetails, NewObject.WorksiteDetails))
                        stringBuilder.AppendLine($" Worksite Detail was modified. Old value: '{obj.WorksiteDetails}', New value: '{NewObject.WorksiteDetails}' '{Environment.NewLine}'");
                    //if (!MatchStringValue(obj.WeekNo, NewObject.WeekNo))
                    //stringBuilder.AppendLine($" Week No was modified. Old value: '{obj.WeekNo}', New value: '{NewObject.WeekNo}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.YardIn, NewObject.YardIn))
                        stringBuilder.AppendLine($" Yard In was modified. Old value: '{obj.YardIn}', New value: '{NewObject.YardIn}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.YardOut, NewObject.YardOut))
                        stringBuilder.AppendLine($" Yard Out was modified. Old value: '{obj.YardOut}', New value: '{NewObject.YardOut}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.ShiftTimeDetail, NewObject.ShiftTimeDetail))
                        stringBuilder.AppendLine($" Shift Time was modified. Old value: '{obj.ShiftTimeDetail}', New value: '{NewObject.ShiftTimeDetail}' '{Environment.NewLine}'");
                    if (!MatchStringValue(obj.PprePlfield, NewObject.PprePlfield))
                        stringBuilder.AppendLine($" PL was modified. Old value: '{obj.PprePlfield}', New value: '{NewObject.PprePlfield}' '{Environment.NewLine}'");
                    obj.Customer = param.Customer;
                    //obj.Ptonumber = param.Ptonumber;
                    obj.PprePlannedHours = param.PprePlannedHours;
                    obj.CnupledMachines = param.CnupledMachines;
                    obj.PprePlannedWork = param.PprePlannedWork;
                    obj.PpreSummary = param.PpreSummary;
                    obj.WonNumber = param.WonNumber;
                    obj.PpreAssessor = param.PpreAssessor;
                    obj.Ppsreference = param.Ppsreference;
                    obj.PpreDay = param.PpreDay;
                    obj.Route = param.Route;
                    obj.FinishTime = param.FinishTime;
                    obj.StartTime = param.StartTime;
                    obj.PpreShiftDate = param.PpreShiftDate;
                    obj.MachineNum = param.MachineNum;
                    //obj.MachineType = param.MachineType;
                    obj.PpreCompany = param.PpreCompany;
                    obj.PpreEmptyComment = param.PpreEmptyComment;
                    obj.PpreFailureComments = param.PpreFailureComments;
                    obj.PpreHeadCode = param.PpreHeadCode;
                    obj.PprePathTime = param.PprePathTime;
                    obj.Shift = param.Shift;
                    obj.PpreAct = param.PpreAct;
                    obj.PpreLogNumber = param.PpreLogNumber;
                    obj.PpreInternalComments = param.PpreInternalComments;
                    obj.StartDate = param.StartDate;
                    obj.FinishDate = param.FinishDate;
                    obj.PpreDra = param.PpreDra;
                    obj.Ptonumber = param.Ptonumber;
                    obj.WorkDescription = param.WorkDescription;
                    obj.WorksiteDescriptor = param.WorksiteDescriptor;
                    obj.WorksiteDetails = param.WorksiteDetails;
                    obj.WeekNo = param.WeekNo;
                    obj.YardIn = param.YardIn;
                    obj.YardOut = param.YardOut;
                    obj.ShiftTimeDetail = param.ShiftTimeDetail;
                    obj.PprePlfield = param.PprePlfield;
                    obj.WorkCompleted = param.WorkCompleted;

                    dBContext.Lnedetails.Update(obj);
                    await dBContext.SaveChangesAsync();


                    if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                    {
                        var objuserl = new UserLog()
                        {
                            ActionType = "Update",
                            AppName = "Shift Detail",
                            UserId = UserId,
                            ShiftId = obj.Id,
                            CreatedDate = DateTime.Now,
                            Description = stringBuilder.ToString()
                        };
                        await dBContext.UserLogs.AddAsync(objuserl);
                        await dBContext.SaveChangesAsync();
                    }

                    return "success";
                }
                else
                {
                    obj = new Lnedetail();

                    param.FinishTime = param.FinishTime + ":00";
                    param.StartTime = param.StartTime + ":00";
                    if (DateTime.TryParseExact(param.FinishTime, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                    {
                        param.FinishTime = dateTime.ToString("h:mm:ss tt");
                    }
                    else
                    {
                        param.FinishTime = "00:00:00 PM";
                    }

                    if (DateTime.TryParseExact(param.StartTime, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime2))
                    {
                        param.StartTime = dateTime2.ToString("h:mm:ss tt");
                    }
                    else
                    {
                        param.StartTime = "00:00:00 PM";
                    }

                    obj.Customer = param.Customer;
                    obj.PprePlannedHours = param.PprePlannedHours;
                    obj.CnupledMachines = param.CnupledMachines;
                    obj.PprePlannedWork = param.PprePlannedWork;
                    obj.PpreShiftDate = param.PpreShiftDate;
                    obj.PpreSummary = param.PpreSummary;
                    obj.WonNumber = param.WonNumber;
                    obj.PpreAssessor = param.PpreAssessor;
                    obj.Ppsreference = param.Ppsreference;
                    obj.PpreDay = param.PpreDay;
                    obj.Route = param.Route;
                    obj.FinishTime = param.FinishTime;
                    obj.StartTime = param.StartTime;
                    obj.PpreShiftDate = param.PpreShiftDate;
                    obj.MachineNum = param.MachineNum;
                    obj.MachineType = param.MachineType;
                    obj.PpreCompany = param.PpreCompany;
                    obj.PpreEmptyComment = param.PpreEmptyComment;
                    obj.PpreFailureComments = param.PpreFailureComments;
                    obj.PpreHeadCode = param.PpreHeadCode;
                    obj.PprePathTime = param.PprePathTime;
                    obj.Shift = param.Shift;
                    obj.PpreAct = param.PpreAct;
                    obj.PpreLogNumber = param.PpreLogNumber;
                    obj.PpreInternalComments = param.PpreInternalComments;
                    obj.StartDate = param.StartDate;
                    obj.FinishDate = param.FinishDate;
                    obj.PpreDra = param.PpreDra;
                    obj.Ptonumber = param.Ptonumber;
                    obj.WorkDescription = param.WorkDescription;
                    obj.WorksiteDescriptor = param.WorksiteDescriptor;
                    obj.WorksiteDetails = param.WorksiteDetails;
                    obj.YardIn = param.YardIn;
                    obj.YardOut = param.YardOut;
                    obj.WeekNo = param.WeekNo;
                    obj.ShiftTimeDetail = param.ShiftTimeDetail;
                    obj.PprePlfield = param.PprePlfield;
                    obj.WorkCompleted = param.WorkCompleted;

                    dBContext.Lnedetails.Add(obj);
                    await dBContext.SaveChangesAsync();

                    var objuserl = new UserLog()
                    {
                        ActionType = "Insert",
                        AppName = "Shift Detail",
                        UserId = UserId,
                        ShiftId = obj.Id,
                        CreatedDate = DateTime.Now,
                        Description = "New Shift Added."
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();

                    return "success";
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        private static Lnedetail AssignOldData(LNEDetailModel param)
        {
            Lnedetail obj = new Lnedetail();
            obj.PprePlannedHours = param.PprePlannedHours;
            obj.CnupledMachines = param.CnupledMachines;
            obj.PprePlannedWork = param.PprePlannedWork;
            obj.PpreSummary = param.PpreSummary;
            obj.WonNumber = param.WonNumber;
            obj.PpreAssessor = param.PpreAssessor;
            obj.Ppsreference = param.Ppsreference;
            obj.PpreDay = param.PpreDay;
            obj.Route = param.Route;
            obj.FinishTime = param.FinishTime;
            obj.StartTime = param.StartTime;
            obj.PpreShiftDate = param.PpreShiftDate;
            obj.MachineNum = param.MachineNum;
            //obj.MachineType = param.MachineType;
            obj.PpreCompany = param.PpreCompany;
            obj.PpreEmptyComment = param.PpreEmptyComment;
            obj.PpreFailureComments = param.PpreFailureComments;
            obj.PpreHeadCode = param.PpreHeadCode;
            obj.PprePathTime = param.PprePathTime;
            obj.Shift = param.Shift;
            obj.PpreAct = param.PpreAct;
            obj.PpreLogNumber = param.PpreLogNumber;
            obj.PpreInternalComments = param.PpreInternalComments;
            obj.StartDate = param.StartDate;
            obj.FinishDate = param.FinishDate;
            obj.PpreDra = param.PpreDra;
            obj.Ptonumber = param.Ptonumber;
            obj.WorkDescription = param.WorkDescription;
            obj.WorksiteDescriptor = param.WorksiteDescriptor;
            obj.WorksiteDetails = param.WorksiteDetails;
            obj.WeekNo = param.WeekNo;
            obj.YardIn = param.YardIn;
            obj.YardOut = param.YardOut;
            obj.ShiftTimeDetail = param.ShiftTimeDetail;
            obj.PprePlfield = param.PprePlfield;

            return obj;
        }

        private static Lnedetail AssignOldDataReal(Lnedetail param)
        {
            Lnedetail obj = new Lnedetail();
            obj.PprePlannedHours = param.PprePlannedHours;
            obj.CnupledMachines = param.CnupledMachines;
            obj.PprePlannedWork = param.PprePlannedWork;
            obj.PpreSummary = param.PpreSummary;
            obj.WonNumber = param.WonNumber;
            obj.PpreAssessor = param.PpreAssessor;
            obj.Ppsreference = param.Ppsreference;
            obj.PpreDay = param.PpreDay;
            obj.Route = param.Route;
            obj.FinishTime = param.FinishTime;
            obj.StartTime = param.StartTime;
            obj.PpreShiftDate = param.PpreShiftDate;
            obj.MachineNum = param.MachineNum;
            //obj.MachineType = param.MachineType;
            obj.PpreCompany = param.PpreCompany;
            obj.PpreEmptyComment = param.PpreEmptyComment;
            obj.PpreFailureComments = param.PpreFailureComments;
            obj.PpreHeadCode = param.PpreHeadCode;
            obj.PprePathTime = param.PprePathTime;
            obj.Shift = param.Shift;
            obj.PpreAct = param.PpreAct;
            obj.PpreLogNumber = param.PpreLogNumber;
            obj.PpreInternalComments = param.PpreInternalComments;
            obj.StartDate = param.StartDate;
            obj.FinishDate = param.FinishDate;
            obj.PpreDra = param.PpreDra;
            obj.Ptonumber = param.Ptonumber;
            obj.WorkDescription = param.WorkDescription;
            obj.WorksiteDescriptor = param.WorksiteDescriptor;
            obj.WorksiteDetails = param.WorksiteDetails;
            obj.WeekNo = param.WeekNo;
            obj.YardIn = param.YardIn;
            obj.YardOut = param.YardOut;
            obj.ShiftTimeDetail = param.ShiftTimeDetail;
            obj.PprePlfield = param.PprePlfield;

            return obj;
        }
        private static bool MatchStringValue(string valueold, string valuenew)
        {
            var IsMatched = true;
            if (string.IsNullOrEmpty(valueold) && !string.IsNullOrEmpty(valuenew))
            {
                IsMatched = false;
            }
            else if (!string.IsNullOrEmpty(valueold) && string.IsNullOrEmpty(valuenew))
            {
                IsMatched = false;
            }
            else if (string.IsNullOrEmpty(valueold) && string.IsNullOrEmpty(valuenew))
            {

            }
            else
            {
                if (valueold?.Trim().ToLower() == valuenew?.Trim().ToLower())
                {

                }
                else
                {
                    IsMatched = false;
                }
            }

            return IsMatched;
        }
        private static bool MatchStringValue(long? valueold, long? valuenew)
        {
            var IsMatched = true;

            if (valueold == valuenew)
            {

            }
            else
            {
                IsMatched = false;
            }


            return IsMatched;
        }
        private static bool MatchStringValue(decimal? valueold, decimal? valuenew)
        {
            var IsMatched = true;

            if (valueold == valuenew)
            {

            }
            else
            {
                IsMatched = false;
            }


            return IsMatched;
        }
        private static bool MatchStringValue(DateTime? dateTimeOld, DateTime? dateTimeNew)
        {
            var IsMatched = true;

            if ((dateTimeOld == null) && (dateTimeNew != null))
            {
                IsMatched = false;
            }
            else if ((dateTimeOld != null) && (dateTimeNew == null))
            {
                IsMatched = false;
            }
            else if ((dateTimeOld == null) && (dateTimeNew == null))
            {

            }
            else
            {
                if (dateTimeOld.Value.Date == dateTimeNew.Value.Date)
                {

                }
                else
                {
                    IsMatched = false;
                }

            }


            return IsMatched;
        }
        public async Task<string> InsertGridMileStone(MileStoneEntry param, string UserId)
        {
            try
            {
                if (param.Id > 0)
                {
                    var obj = await dBContext.MileStoneEntries.FindAsync(param.Id);
                    if (obj != null)
                    {
                        if (obj.LogEntry == "TopLine")
                        {
                            obj.MileStoneEntryDetail = param.MileStoneEntryDetail;
                        }
                        obj.Planned = param.Planned;
                        obj.Actuall = param.Actuall;
                        obj.Comments = param.Comments;
                        dBContext.MileStoneEntries.Update(obj);
                       // dBContext.MileStoneEntries.Add(obj);

                        await dBContext.SaveChangesAsync();
                    }
                    var objuserl = new UserLog()
                    {
                        ActionType = "Update",
                        AppName = "Milestone",
                        UserId = UserId,
                        CreatedDate = DateTime.Now,
                        ShiftId = param.ShiftId,
                        Description = "Milestone activity updated."
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();

                    return "Updated";
                }
                else
                {

                    dBContext.MileStoneEntries.Add(param);
                    await dBContext.SaveChangesAsync();
                    var objuserl = new UserLog()
                    {
                        ActionType = "Insert",
                        AppName = "Milestone",
                        UserId = UserId,
                        CreatedDate = DateTime.Now,
                        ShiftId = param.ShiftId,
                        Description = "Milestone activity addedd."
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();
                    return "Saved";
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

        public async Task<List<ShiftDetailEmployeeModel>> InsertPersons(ShiftDetailEmployeeModel param, string UserId)
        {
            try
            {
                string EmployeeType = "";
                if (param.EmployeeType == "GridCrewManager")
                    EmployeeType = "Crew";
                if (param.EmployeeType == "GridConductor")
                    EmployeeType = "Conductor";
                if (param.EmployeeType == "GridDriver")
                    EmployeeType = "Driver";
                if (param.EmployeeType == "GridAssesor")
                    EmployeeType = "Assessor";

                string descirption = "";
                var EmployeeName = await dBContext.Employees.Where(x => x.Id == param.EmployeeId).FirstOrDefaultAsync();
                if (param.Id > 0)
                {
                    var obj = await dBContext.ShiftDetailEmployees.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.ContactNumber = param.ContactNumber;
                        obj.EmployeeType = param.EmployeeType;
                        obj.ShiftId = param.ShiftId;
                        obj.EmployeeId = param.EmployeeId;
                        obj.JobTitle = param.JobTitle;

                        dBContext.ShiftDetailEmployees.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }
                    descirption = "(" + EmployeeName?.FirstName + " " + EmployeeName?.LastName + ") name is linked as a " + EmployeeType + " in this shift";
                    var objuserl = new UserLog()
                    {
                        ActionType = "Update",
                        AppName = "Insert " + EmployeeType,
                        UserId = UserId,
                        CreatedDate = DateTime.Now,
                        ShiftId = param.ShiftId,
                        RecordStatus = "Active",
                        Description = descirption
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();

                }
                else
                {
                    ShiftDetailEmployee obj = new ShiftDetailEmployee()
                    {
                        ShiftId = param.ShiftId,
                        RecordStatus = "Active",
                        ContactNumber = param.ContactNumber,
                        EmployeeId = param.EmployeeId,
                        EmployeeType = param.EmployeeType,
                        JobTitle = param.JobTitle
                    };

                    dBContext.ShiftDetailEmployees.Add(obj);
                    await dBContext.SaveChangesAsync();

                    descirption = "(" + EmployeeName?.FirstName + " " + EmployeeName?.LastName + ") name is linked as a " + EmployeeType + " in this shift";
                    var objuserl = new UserLog()
                    {
                        ActionType = "Insert",
                        AppName = "Insert " + EmployeeType,
                        UserId = UserId,
                        CreatedDate = DateTime.Now,
                        ShiftId = param.ShiftId,
                        RecordStatus = "Active",
                        Description = descirption
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();

                }

                var employeeObject = await (from emp in dBContext.Employees
                                            join shiftemp in dBContext.ShiftDetailEmployees on emp.Id equals shiftemp.EmployeeId
                                            where shiftemp.ShiftId == param.ShiftId
                                            select new ShiftDetailEmployeeModel
                                            {
                                                EmployeeId = shiftemp.EmployeeId ?? 0,
                                                ContactNumber = shiftemp.ContactNumber ?? "",
                                                Id = shiftemp.Id,
                                                JobTitle = shiftemp.JobTitle ?? "",
                                                EmployeeType = shiftemp.EmployeeType ?? "",
                                                FullName = emp.FirstName + " " + emp.LastName
                                            }).ToListAsync();

                return employeeObject;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        public async Task<string> CheckIfExist(ShiftDetailEmployeeModel param, string UserId)
        {
            try
            {
                var employeshift = await dBContext.ShiftDetailEmployees.Where(x => x.EmployeeId == param.EmployeeId).Select(x => x.ShiftId).ToListAsync();
                var shiftData = await dBContext.Lnedetails.Where(x => x.Id == param.ShiftId).FirstOrDefaultAsync();
                if (shiftData != null)
                {
                    if (employeshift?.Count > 0)
                    {
                        var shiftDataOr = await dBContext.Lnedetails.Where(x => employeshift.Contains(x.Id)).ToListAsync();
                        if (shiftDataOr != null)
                        {
                            bool BetweenTimeExist = false;
                            bool IsLess12Hours = false;
                            var currentShiftDateTime = CombineDateTime(shiftData.StartDate.Value, shiftData.StartTime);
                            foreach (var shift in shiftDataOr)
                            {
                                var StartDateTime = CombineDateTime(shift.StartDate.Value, shift.StartTime);
                                var EndDateTime = CombineDateTime(shift.FinishDate.Value, shift.FinishTime);
                                if (currentShiftDateTime >= StartDateTime && currentShiftDateTime <= EndDateTime)
                                {
                                    BetweenTimeExist = true;
                                }

                                if (currentShiftDateTime > EndDateTime)
                                {
                                    TimeSpan timeDifference = currentShiftDateTime - EndDateTime;
                                    if (timeDifference.TotalHours <= 12)
                                    {
                                        IsLess12Hours = true;
                                    }
                                }
                            }

                          
                            if (IsLess12Hours)
                                return "ShiftExist12";
                            if (BetweenTimeExist)
                                return "ShiftExist";
                        }
                       
                    }

                
                }


                var objSameDay = await dBContext.Absances.Where(x => (shiftData.StartDate.Value.Date >= x.DateFrom.Value.Date && shiftData.StartDate.Value.Date <= x.DateTo.Value.Date) && x.EmployeeId == param.EmployeeId).FirstOrDefaultAsync();
                if(objSameDay != null)
                {
                    return "LeaveExist";
                }
                var obj = await dBContext.Absances.Where(x => (shiftData.StartDate.Value.Date >= x.DateFrom.Value.Date.AddDays(1) && shiftData.StartDate.Value.Date <= x.DateTo.Value.Date.AddDays(1)) && x.EmployeeId == param.EmployeeId).FirstOrDefaultAsync();
                if (obj != null)
                {
                    var StartDateTime = CombineDateTime(shiftData.StartDate.Value, shiftData.StartTime);
                    var EndDateTime = CombineDateTime(shiftData.FinishDate.Value, shiftData.FinishTime);

                    TimeSpan timeDifference = obj.DateTo.Value.AddDays(1).AddHours(6) - StartDateTime;
                    TimeSpan timeDifferenceEnd = obj.DateTo.Value.AddDays(1).AddHours(6) - EndDateTime;
                    if(
                        (timeDifference.TotalHours > 0 && timeDifference.TotalHours <= 6) ||
                        (timeDifferenceEnd.TotalHours > 0 && timeDifferenceEnd.TotalHours <= 6)
                      )
                    {
                        return "LeaveExist";
                    }                   

                }
              
               
                return "NoExist";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public static DateTime CombineDateTime(DateTime date, string time)
        {
            //string[] formats = { "hh:mm:ss tt", "h:mm:ss tt", "hh:mm:ss" };
            var timepars = DateTime.Parse(time).TimeOfDay;
            return date.Date.Add(timepars);
        }
        public async Task<string> InsertShiftContact(ShiftContact param, string UserId)
        {
            try
            {
                var obj = await dBContext.ShiftContacts.Where(x => x.ContactType == param.ContactType && x.Id != param.Id && x.ShiftId == param.ShiftId).FirstOrDefaultAsync();
                if (obj != null)
                {
                    return "Contract type is already exist.";
                }

                if (param.Id > 0)
                {
                    obj = await dBContext.ShiftContacts.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.ShiftId = param.ShiftId;
                        obj.ContactId = param.ContactId;
                        obj.ContactType = param.ContactType;


                        dBContext.ShiftContacts.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }
                    var objuserl = new UserLog()
                    {
                        ActionType = "Update",
                        AppName = "Shift Contact",
                        UserId = UserId,
                        RecordStatus = "Active",
                        CreatedDate = DateTime.Now,
                        Description = obj.ContactType + " Updated with shift"
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();
                    return "Updated";
                }
                else
                {

                    dBContext.ShiftContacts.Add(param);
                    await dBContext.SaveChangesAsync();
                    var objuserl = new UserLog()
                    {
                        ActionType = "Insert",
                        AppName = "Shift Contact",
                        UserId = UserId,
                        RecordStatus = "Active",
                        CreatedDate = DateTime.Now,
                        Description = param.ContactType + " linked with shift"
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();
                    return "Saved";
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public async Task<bool> deleteGridMileStone(int Id, string UserId)
        {
            try
            {
                var obj = dBContext.MileStoneEntries.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.MileStoneEntries.Remove(obj);
                    await dBContext.SaveChangesAsync();

                    var objuserl = new UserLog()
                    {
                        ActionType = "Deleted",
                        AppName = "Milestone",
                        CreatedDate = DateTime.Now,
                        ShiftId = obj.ShiftId,
                        RecordStatus = "Active",
                        Description = obj.LogEntry + " Milestone activity deleted from shift.",
                        UserId = UserId
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
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
        public async Task<bool> deletePersons(int Id, string UserId)
        {
            try
            {

                var obj = dBContext.ShiftDetailEmployees.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.ShiftDetailEmployees.Remove(obj);
                    await dBContext.SaveChangesAsync();

                    string EmployeeType = "";
                    if (obj.EmployeeType == "GridCrewManager")
                        EmployeeType = "Crew";
                    if (obj.EmployeeType == "GridConductor")
                        EmployeeType = "Conductor";
                    if (obj.EmployeeType == "GridDriver")
                        EmployeeType = "Driver";
                    if (obj.EmployeeType == "GridAssesor")
                        EmployeeType = "Assessor";
                    string descirption = "";
                    var EmployeeName = await dBContext.Employees.Where(x => x.Id == obj.EmployeeId).FirstOrDefaultAsync();
                    descirption = "(" + EmployeeName?.FirstName + " " + EmployeeName?.LastName + ") name is deleted as a " + EmployeeType + " in this shift";



                    var objuserl = new UserLog()
                    {
                        ActionType = "Deleted",
                        AppName = "Deleted " + EmployeeType,
                        CreatedDate = DateTime.Now,
                        ShiftId = obj.ShiftId,
                        RecordStatus = "Active",
                        Description = descirption,
                        UserId = UserId
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
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
        public async Task<bool> deleteShiftContact(int Id, string UserId)
        {
            try
            {
                var obj = dBContext.ShiftContacts.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.ShiftContacts.Remove(obj);
                    await dBContext.SaveChangesAsync();


                    var objuserl = new UserLog()
                    {
                        ActionType = "Deleted",
                        AppName = "Deleted ",
                        CreatedDate = DateTime.Now,
                        ShiftId = obj.ShiftId,
                        RecordStatus = "Active",
                        Description = obj.ContactType + " deleted from shift",
                        UserId = UserId
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
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
        public async Task<bool> delete(int Id)
        {
            try
            {
                var obj = dBContext.Lnedetails.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.Lnedetails.Remove(obj);
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
        public async Task<List<MileStoneEntry>> GetLogTypes(string type, int shiftid, string UserId)
        {
            try
            {

                var ExistList = await dBContext.MileStoneEntries.Where(x => x.ShiftId == shiftid && x.LogEntry!= "TopLine").ToListAsync();
                if (ExistList != null)
                {
                    dBContext.MileStoneEntries.RemoveRange(ExistList);
                    await dBContext.SaveChangesAsync();
                }

                if (type != "Clear")
                {
                    var list = await dBContext.LogTypeEntries.Where(x => x.LogType == type || x.LogType == "TopLine").ToListAsync();
                    if (list != null)
                    {

                        List<MileStoneEntry> mileStoneEntries = new List<MileStoneEntry>();
                        foreach (var entry in list)
                        {
                            var milestone = new MileStoneEntry()
                            {
                                ShiftId = shiftid,
                                MileStoneEntryDetail = entry.LogName,
                                Actuall = "",
                                Planned = "",
                                LogEntry = entry.LogType,
                                Comments = "",
                                RecordStatus = "Active",
                            };
                            dBContext.MileStoneEntries.Add(milestone);
                            await dBContext.SaveChangesAsync();
                        }

                        var objuserl = new UserLog()
                        {
                            ActionType = "Added",
                            AppName = "Milestone",
                            UserId = UserId,
                            RecordStatus = "Active",
                            CreatedDate = DateTime.Now,
                            ShiftId = shiftid,
                            Description = "Milestone activity added."
                        };
                        await dBContext.UserLogs.AddAsync(objuserl);
                        await dBContext.SaveChangesAsync();

                    }
                }
                else
                {

                    var objuserl = new UserLog()
                    {
                        ActionType = "Cleared",
                        AppName = "Milestone",
                        UserId = UserId,
                        RecordStatus = "Active",
                        CreatedDate = DateTime.Now,
                        ShiftId = shiftid,
                        Description = "Milestone activity cleared."
                    };
                    await dBContext.UserLogs.AddAsync(objuserl);
                    await dBContext.SaveChangesAsync();
                }
                //var listMilestone = await dBContext.MileStoneEntries.Where(x => x.ShiftId == shiftid && x.RecordStatus == "Active")
                //    .OrderBy(item => item.LogEntry != "TopLine").ThenBy(item => item.LogEntry).ToListAsync();

                var listMilestone = await dBContext.MileStoneEntries.Where(x => x.ShiftId == shiftid && x.RecordStatus == "Active" && x.LogEntry != "TopLine").ToListAsync();
                var topline = await dBContext.MileStoneEntries.Where(x => x.ShiftId == shiftid && x.RecordStatus == "Active" && x.LogEntry == "TopLine").FirstOrDefaultAsync();
                if (topline != null)
                {
                    listMilestone.Insert(0, topline);
                    // listMilestone.Prepend(topline);
                }

                return listMilestone;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<string> InsertBulkData(List<Lnedetail> paramList, string UserId)
        {
            try
            {
                List<string> ptoNumbers = new List<string>();
                ptoNumbers = paramList.Select(x => x.Ptonumber ?? "").ToList();

                if (ptoNumbers?.Count > 0)
                {
                    var listExists = await dBContext.Lnedetails.Where(x => ptoNumbers.Contains(x.Ptonumber ?? "")).ToListAsync();
                    if (listExists?.Count > 0)
                    {
                        foreach (var obj in listExists)
                        {



                            var param = paramList.Where(x => x.Ptonumber == obj.Ptonumber).FirstOrDefault();
                            if (!string.IsNullOrEmpty(param.Ptonumber))
                            {
                                var fristThree = param.Ptonumber.Substring(0, Math.Min(3, param.Ptonumber.Length));
                                if (fristThree.ToUpper() == "OTM")
                                {
                                    param.PpreAct = "W";
                                }
                            }

                            if (obj != null)
                            {



                                var NewObject = AssignOldDataReal(param);
                                // List<ModifiedPropertiesModel> modifiedProperties = GetModifiedProperties(obj, NewObject);

                                StringBuilder stringBuilder = new StringBuilder();
                                // if (MatchStringValue(obj.PprePlannedHours, NewObject.PprePlannedHours))
                                //    stringBuilder.AppendLine($"Property Planned Hours was modified. Old value: '{obj.PprePlannedHours}', New value: '{NewObject.PprePlannedHours}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.CnupledMachines, NewObject.CnupledMachines))
                                    stringBuilder.AppendLine($" CnupledMachines  was modified. Old value: '{obj.CnupledMachines}', New value: '{NewObject.CnupledMachines}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PprePlannedWork, NewObject.PprePlannedWork))
                                    stringBuilder.AppendLine($" Planned Work was modified. Old value: '{obj.PprePlannedWork}', New value: '{NewObject.PprePlannedWork}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreSummary, NewObject.PpreSummary))
                                    stringBuilder.AppendLine($" Summary was modified. Old value: '{obj.PpreSummary}', New value: '{NewObject.PpreSummary}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.WonNumber, NewObject.WonNumber))
                                    stringBuilder.AppendLine($" Won Number was modified. Old value: '{obj.WonNumber}', New value: '{NewObject.WonNumber}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreAssessor, NewObject.PpreAssessor))
                                    stringBuilder.AppendLine($" Assessor was modified. Old value: '{obj.PpreAssessor}', New value: '{NewObject.PpreAssessor}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.Ppsreference, NewObject.Ppsreference))
                                    stringBuilder.AppendLine($"Worksite Refernce was modified. Old value: '{obj.Ppsreference}', New value: '{NewObject.Ppsreference}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreDay, NewObject.PpreDay))
                                    stringBuilder.AppendLine($" Day was modified. Old value: '{obj.PpreDay}', New value: '{NewObject.PpreDay}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.Route, NewObject.Route))
                                    stringBuilder.AppendLine($"Route was modified. Old value: '{obj.Route}', New value: '{NewObject.Route}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.FinishTime, NewObject.FinishTime))
                                    stringBuilder.AppendLine($"Finish Time was modified. Old value: '{obj.FinishTime}', New value: '{NewObject.FinishTime}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.StartTime, NewObject.StartTime))
                                    stringBuilder.AppendLine($" Start Time was modified. Old value: '{obj.StartTime}', New value: '{NewObject.StartTime}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreShiftDate, NewObject.PpreShiftDate))
                                    stringBuilder.AppendLine($" Shift Date was modified. Old value: '{obj.PpreShiftDate}', New value: '{NewObject.PpreShiftDate}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.MachineNum, NewObject.MachineNum))
                                    stringBuilder.AppendLine($" Machine Number was modified. Old value: '{obj.MachineNum}', New value: '{NewObject.MachineNum}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreCompany, NewObject.PpreCompany))
                                    stringBuilder.AppendLine($" Company was modified. Old value: '{obj.PpreCompany}', New value: '{NewObject.PpreCompany}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreEmptyComment, NewObject.PpreEmptyComment))
                                    stringBuilder.AppendLine($" Comment was modified. Old value: '{obj.PpreEmptyComment}', New value: '{NewObject.PpreEmptyComment}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreFailureComments, NewObject.PpreFailureComments))
                                    stringBuilder.AppendLine($" Failure Comments was modified. Old value: '{obj.PpreFailureComments}', New value: '{NewObject.PpreFailureComments}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreHeadCode, NewObject.PpreHeadCode))
                                    stringBuilder.AppendLine($" Head Code was modified. Old value: '{obj.PpreHeadCode}', New value: '{NewObject.PpreHeadCode}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PprePathTime, NewObject.PprePathTime))
                                    stringBuilder.AppendLine($" Path Time was modified. Old value: '{obj.PprePathTime}', New value: '{NewObject.PprePathTime}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.Shift, NewObject.Shift))
                                    stringBuilder.AppendLine($" Shift Status was modified. Old value: '{obj.Shift}', New value: '{NewObject.Shift}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreAct, NewObject.PpreAct))
                                    stringBuilder.AppendLine($" ACT was modified. Old value: '{obj.PpreAct}', New value: '{NewObject.PpreAct}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreLogNumber, NewObject.PpreLogNumber))
                                    stringBuilder.AppendLine($" Log Number was modified. Old value: '{obj.PpreLogNumber}', New value: '{NewObject.PpreLogNumber}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreInternalComments, NewObject.PpreInternalComments))
                                    stringBuilder.AppendLine($" Internal Comments was modified. Old value: '{obj.PpreInternalComments}', New value: '{NewObject.PpreInternalComments}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.StartDate, NewObject.StartDate))
                                    stringBuilder.AppendLine($" Start Date was modified. Old value: '{obj.StartDate}', New value: '{NewObject.StartDate}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.FinishDate, NewObject.FinishDate))
                                    stringBuilder.AppendLine($" Finish Date was modified. Old value: '{obj.FinishDate}', New value: '{NewObject.FinishDate}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PpreDra, NewObject.PpreDra))
                                    stringBuilder.AppendLine($" DRA was modified. Old value: '{obj.PpreDra}', New value: '{NewObject.PpreDra}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.Ptonumber, NewObject.Ptonumber))
                                    stringBuilder.AppendLine($" PTO Number was modified. Old value: '{obj.Ptonumber}', New value: '{NewObject.Ptonumber}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.WorkDescription, NewObject.WorkDescription))
                                    stringBuilder.AppendLine($" Work Description was modified. Old value: '{obj.WorkDescription}', New value: '{NewObject.WorkDescription}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.WorksiteDescriptor, NewObject.WorksiteDescriptor))
                                    stringBuilder.AppendLine($" Worksite Descriptor was modified. Old value: '{obj.WorksiteDescriptor}', New value: '{NewObject.WorksiteDescriptor}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.WorksiteDetails, NewObject.WorksiteDetails))
                                    stringBuilder.AppendLine($" Worksite Detail was modified. Old value: '{obj.WorksiteDetails}', New value: '{NewObject.WorksiteDetails}' '{Environment.NewLine}'");
                                //if (!MatchStringValue(obj.WeekNo, NewObject.WeekNo))
                                //stringBuilder.AppendLine($" Week No was modified. Old value: '{obj.WeekNo}', New value: '{NewObject.WeekNo}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.YardIn, NewObject.YardIn))
                                    stringBuilder.AppendLine($" Yard In was modified. Old value: '{obj.YardIn}', New value: '{NewObject.YardIn}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.YardOut, NewObject.YardOut))
                                    stringBuilder.AppendLine($" Yard Out was modified. Old value: '{obj.YardOut}', New value: '{NewObject.YardOut}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.ShiftTimeDetail, NewObject.ShiftTimeDetail))
                                    stringBuilder.AppendLine($" Shift Time was modified. Old value: '{obj.ShiftTimeDetail}', New value: '{NewObject.ShiftTimeDetail}' '{Environment.NewLine}'");
                                if (!MatchStringValue(obj.PprePlfield, NewObject.PprePlfield))
                                    stringBuilder.AppendLine($" PL was modified. Old value: '{obj.PprePlfield}', New value: '{NewObject.PprePlfield}' '{Environment.NewLine}'");


                                obj.OrderRevision = param.OrderRevision;
                                obj.PpreAct = param.PpreAct;
                                obj.JobNumber = param.JobNumber;
                                obj.WeekNo = param.WeekNo;
                                obj.StartDate = param.StartDate;
                                obj.Shift = "Live";
                                obj.TrainOrderType = param.TrainOrderType;
                                obj.MachineType = param.MachineType;
                                obj.WorksiteDetails = param.WorksiteDetails;
                                obj.Ppsreference = param.WorksiteNo;
                                obj.StartTime = param.StartTime;
                                obj.FinishDate = param.FinishDate;
                                obj.FinishTime = param.FinishTime;
                                obj.Mileage = param.Mileage;
                                obj.YardOut = param.YardOut;
                                obj.YardIn = param.YardIn;
                                obj.LineToArriveOn = param.LineToArriveOn;
                                obj.WorkDescription = param.WorkDescription;
                                obj.WorksiteDescriptor = param.WorksiteDescriptor;
                                obj.ConsistSleeperType = param.ConsistSleeperType;
                                obj.Supplier = param.Supplier;
                                obj.MachineNum = param.MachineNum;
                                obj.Contractor = param.Contractor;
                                obj.Customer = param.Customer;
                                obj.Route = param.Route;
                                obj.OrderStatus = param.OrderStatus;
                                obj.WorksiteNo = param.WorksiteNo;
                                obj.Remarks = param.Remarks;
                                obj.OrderDate = param.OrderDate;
                                obj.EnteredBy = param.EnteredBy;
                                obj.CancellationDate = param.CancellationDate;
                                obj.CancelledBy = param.CancelledBy;
                                obj.PossessionDetails = param.PossessionDetails;
                                obj.WorksiteElr = param.WorksiteElr;
                                obj.WorkYardage = param.WorkYardage;
                                obj.PartOfBlockade = param.PartOfBlockade;
                                obj.PriorityStatusShift = param.PriorityStatusShift;
                                obj.MachineSupply = param.MachineSupply;
                                obj.CnupledMachines = param.CnupledMachines;
                                obj.TrainId = param.TrainId;
                                obj.TimeOutOfYard = param.TimeOutOfYard;
                                obj.TimeBackToYard = param.TimeBackToYard;
                                obj.PprePlannedWork = param.Remarks;
                                obj.PprePlfield = param.WorkDescription;
                                obj.WonNumber = param.WonNumber;

                                dBContext.Lnedetails.Update(obj);
                                await dBContext.SaveChangesAsync();

                                if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                                {
                                    var objuserld = new UserLog()
                                    {
                                        ActionType = "Update",
                                        AppName = "Shift Detail Import",
                                        UserId = UserId,
                                        ShiftId = obj.Id,
                                        CreatedDate = DateTime.Now,
                                        Description = stringBuilder.ToString()
                                    };
                                    await dBContext.UserLogs.AddAsync(objuserld);
                                    await dBContext.SaveChangesAsync();
                                }
                            }
                        }

                        ptoNumbers = listExists.Select(x => x.Ptonumber).ToList();
                    }
                    else
                    {
                        ptoNumbers = new List<string>();
                    }
                }
                if (ptoNumbers?.Count > 0)
                {
                    paramList = paramList.Where(x => !ptoNumbers.Contains(x.Ptonumber)).ToList();
                }
                if (paramList?.Count > 0)
                {
                    foreach (var lnedetail in paramList)
                    {
                        lnedetail.PprePlannedWork = lnedetail.Remarks;
                        lnedetail.Shift = "Live";
                        lnedetail.PprePlfield = lnedetail.WorkDescription;
                        if (!string.IsNullOrEmpty(lnedetail.Ptonumber))
                        {
                            var fristThree = lnedetail.Ptonumber.Substring(0, Math.Min(3, lnedetail.Ptonumber.Length));
                            if (fristThree.ToUpper() == "OTM")
                            {
                                lnedetail.PpreAct = "W";
                            }
                        }
                        //var loObj = await dBContext.Routes.Where(x => x.Name == lnedetail.WorksiteDetails).FirstOrDefaultAsync();
                        //if (loObj == null)
                        //{
                        //    var routObject = new Models.Route()
                        //    {
                        //        Name = lnedetail.WorksiteDetails,
                        //        ShortCode = lnedetail.WorksiteDetails,
                        //        StablingPoints = false,
                        //        RecordStatus = "Active",
                        //        CreatedBy = UserId,
                        //        CreatedDate = DateTime.Now
                        //    };

                        //    await dBContext.Routes.AddAsync(routObject);
                        //    await dBContext.SaveChangesAsync();
                        //}
                    }
                    //paramList.ForEach(item => item.PprePlannedWork = item.Remarks);
                    await dBContext.Lnedetails.AddRangeAsync(paramList);
                    await dBContext.SaveChangesAsync();
                }

                var objuserl = new UserLog()
                {
                    ActionType = "Import",
                    AppName = "Shift Detail",
                    UserId = UserId,
                    CreatedDate = DateTime.Now
                };
                await dBContext.UserLogs.AddAsync(objuserl);
                await dBContext.SaveChangesAsync();


                return "success";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }


        public async Task<DriverQuestion> GetDriverQuestion(int id, int driverId)
        {
            try
            {
                var employeeObject = await dBContext.DriverQuestions.Where(x => x.DriverId == driverId && x.ShiftId == id).FirstOrDefaultAsync();
                if (employeeObject == null)
                {
                    employeeObject = new DriverQuestion()
                    {
                        ShiftId = id,
                        IsSetisfied = "",
                        Comments = "",
                        DriverId = driverId,
                        Question1 = null,
                        Question10 = null,
                        Question2 = null,
                        Question3 = null,
                        Question4 = null,
                        Question5 = null,
                        Question6 = null,
                        Question7 = null,
                        Question8 = null,
                        Question9 = null
                    };
                }
                return employeeObject;

            }
            catch (Exception ex)
            {
                return null;

            }
        }
        public async Task<string> SaveDriverQuestion(DriverQuestion param)
        {
            try
            {
                if (param.Id > 0)
                {
                    var obj = await dBContext.DriverQuestions.FindAsync(param.Id);
                    if (obj != null)
                    {

                        obj.Question6 = param.Question6;
                        obj.Question7 = param.Question7;
                        obj.Question8 = param.Question8;
                        obj.Question5 = param.Question5;
                        obj.Question4 = param.Question4;
                        obj.Question3 = param.Question3;
                        obj.Question2 = param.Question2;
                        obj.Question1 = param.Question1;
                        //obj.Question9 = param.Question9;
                        obj.Question10 = param.Question10;
                        obj.IsSetisfied = param.IsSetisfied;
                        obj.Comments = param.Comments;

                        dBContext.DriverQuestions.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }

                    return "Updated";
                }
                else
                {

                    dBContext.DriverQuestions.Add(param);
                    await dBContext.SaveChangesAsync();

                    return "Saved";
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public async Task<string> InsertVSTP(MachineShiftVstp param)
        {
            try
            {


                param.ModifiedDate = DateTime.Now;
                dBContext.MachineShiftVstps.Add(param);
                await dBContext.SaveChangesAsync();

                return "Updated";

            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public async Task<string> SaveShiftStatus(string Status, List<int> Ids)
        {
            try
            {

                foreach (int i in Ids)
                {
                    var obj = dBContext.Lnedetails.Where(x => x.Id == i).FirstOrDefault();
                    if (obj != null)
                    {

                        obj.Shift = Status;
                        dBContext.Lnedetails.Update(obj);
                        await dBContext.SaveChangesAsync();

                    }
                }

                return "Updated";

            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public async Task<FilterHistory> GetFilterHistory(string UserId)
        {
            try
            {
                var employeeObject = await dBContext.FilterHistories.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
                return employeeObject;

            }
            catch (Exception ex)
            {
                return null;

            }
        }
        public async Task<Lnedetail> DuplicateRecords(List<int> Ids, bool IsHoursAdded)
        {
            Lnedetail newObj = null;
            try
            {


                foreach (int i in Ids)
                {
                    var obj = dBContext.Lnedetails.Where(x => x.Id == i).FirstOrDefault();
                    if (obj != null)
                    {                                               
                            newObj = new Lnedetail()
                            {
                                Shift = obj.Shift,
                                StartDate = IsHoursAdded ? Convert.ToDateTime(obj.StartDate).AddHours(24) : obj.StartDate,
                                StartTime = obj.StartTime,
                                Supplier = obj.Supplier,
                                ConsistSleeperType = obj.ConsistSleeperType,
                                MachineSupply = obj.MachineSupply,
                                OrderStatus = obj.OrderStatus,
                                PpreShiftDate = obj.PpreShiftDate,
                                PpreSummary = obj.PpreSummary,
                                PriorityStatusShift = obj.PriorityStatusShift,
                                CancellationDate = obj.CancellationDate,
                                CancelledBy = obj.CancelledBy,
                                CnupledMachines = obj.CnupledMachines,
                                Contractor = obj.Contractor,
                                Customer = obj.Customer,
                                EnteredBy = obj.EnteredBy,
                                FinishDate = IsHoursAdded ? Convert.ToDateTime(obj.FinishDate).AddHours(24) : obj.FinishDate,
                                FinishTime = obj.FinishTime,
                                JobNumber = obj.JobNumber,
                                LineToArriveOn = obj.LineToArriveOn,
                                MachineNum = obj.MachineNum,
                                MachineType = obj.MachineType,
                                Mileage = obj.Mileage,
                                OrderDate = obj.OrderDate,
                                OrderRevision = obj.OrderRevision,
                                PartOfBlockade = obj.PartOfBlockade,
                                PossessionDetails = obj.PossessionDetails,
                                PpreAct = obj.PpreAct,
                                PpreAssessor = obj.PpreAssessor,
                                PpreCompany = obj.PpreCompany,
                                PpreDay = obj.PpreDay,
                                PpreDra = obj.PpreDra,
                                PpreDriver = obj.PpreDriver,
                                PpreEmptyComment = obj.PpreEmptyComment,
                                PpreFailureComments = obj.PpreFailureComments,
                                PpreHeadCode = obj.PpreHeadCode,
                                PpreInternalComments = obj.PpreInternalComments,
                                PpreLogNumber = obj.PpreLogNumber,
                                PpreOperator = obj.PpreOperator,
                                PprePathTime = obj.PprePathTime,
                                PprePlannedHours = obj.PprePlannedHours,
                                PprePlannedWork = obj.PprePlannedWork,
                                WonNumber = obj.WonNumber,
                                Ppsreference = obj.Ppsreference,
                                //Ptonumber = obj.Ptonumber,
                                Ptonumber = string.Empty,
                                PprePlfield = obj.PprePlfield,
                                Remarks = obj.Remarks,
                                Route = obj.Route,
                                TimeBackToYard = obj.TimeBackToYard,
                                TimeOutOfYard = obj.TimeOutOfYard,
                                TrainId = obj.TrainId,
                                TrainOrderType = obj.TrainOrderType,
                                WeekNo = obj.WeekNo,
                                WorkDescription = obj.WorkDescription,
                                WorksiteDescriptor = obj.WorksiteDescriptor,
                                WorksiteDetails = obj.WorksiteDetails,
                                WorksiteElr = obj.WorksiteElr,
                                WorksiteNo = obj.WorksiteNo,
                                WorkYardage = obj.WorkYardage,
                                YardIn = obj.YardIn,
                                YardOut = obj.YardOut
                            };
                        
                        
                                                                
                        dBContext.Lnedetails.Add(newObj);
                        await dBContext.SaveChangesAsync();

                    }
                }

                return newObj;

            }
            catch (Exception ex)
            {

                return null;
            }

        }
        public async Task<WeeklyCommentsModel> GetWeeklyComments(int WeekId)
        {
            try
            {
                WeeklyCommentsModel resultObj = new WeeklyCommentsModel();
                var obj = await dBContext.WeeklyComments.Where(x => x.WeekId == WeekId).FirstOrDefaultAsync();
                if (obj != null)
                {
                    resultObj.WeekId = obj.WeekId;
                    resultObj.CoursesAndOthers = obj.CoursesAndOthers;
                    resultObj.EngineeringSupport = obj.EngineeringSupport;
                    resultObj.Id = obj.Id;
                    //var ListArrangements = new List<WeekArrangement>();
                    var ListArrangements = await dBContext.WeekArrangements.Where(x => x.WeekId == WeekId).ToListAsync();
                    resultObj.ListArrangements = ListArrangements;

                }
                return resultObj;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        public async Task<string> SaveWeeklyComments(WeeklyCommentsModel param)
        {
            try
            {
                var obj = await dBContext.WeeklyComments.Where(x => x.WeekId == param.WeekId).FirstOrDefaultAsync();
                if (obj != null)
                {
                    obj.EngineeringSupport = param.EngineeringSupport;
                    //obj.CoursesAndOthers = param.CoursesAndOthers;
                    obj.CoursesAndOthers = param.CoursesAndOthers;
                    dBContext.WeeklyComments.Update(obj);
                    await dBContext.SaveChangesAsync();

                    var listArran = await dBContext.WeekArrangements.Where(x => x.WeekId == param.WeekId).ToListAsync();
                    dBContext.WeekArrangements.RemoveRange(listArran);
                    await dBContext.SaveChangesAsync();



                }
                else
                {
                    obj = new WeeklyComment()
                    {
                        EngineeringSupport = param.EngineeringSupport,
                        CoursesAndOthers = param.CoursesAndOthers,
                        CreatedDate = DateTime.Now,
                        UserId = param.UserId,
                        WeekId = param.WeekId
                    };
                    dBContext.WeeklyComments.Add(obj);
                    await dBContext.SaveChangesAsync();


                }


                if (param.ListArrangements?.Count > 0)
                {
                    foreach (var item in param.ListArrangements)
                    {
                        WeekArrangement itemobj = new WeekArrangement()
                        {
                            ColumnNo1 = item.ColumnNo1,
                            ColumnNo2 = item.ColumnNo2,
                            ColumnNo3 = item.ColumnNo3,
                            ColumnNo4 = item.ColumnNo4,
                            ColumnNo5 = item.ColumnNo5,
                            ColumnNo6 = item.ColumnNo6,
                            ColumnNo7 = item.ColumnNo7,
                            ColumnNo8 = item.ColumnNo8,
                            UserId = param.UserId,
                            WeekId = param.WeekId
                        };
                        dBContext.WeekArrangements.Add(itemobj);
                        await dBContext.SaveChangesAsync();
                    }
                }


                return "Saved";

            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

        public async Task<string> ResetFilters(string UserId)
        {
            try
            {
                var obj = await dBContext.FilterHistories.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
                if (obj != null)
                {
                    obj.Act = false;
                    obj.Template = null;
                    obj.ToWeek = null;
                    obj.FromWeek = null;
                    obj.Ott = false;
                    obj.Otpm = false;
                    obj.MachineStatus = null;
                    obj.ShiftStatus = null;
                    obj.ShiftCancelled = false;
                    obj.ShiftCaped = false;
                    obj.ShiftBlank = false;
                    obj.MachineNumber = null;
                    obj.Owner = false;
                    obj.Otm = false;

                    dBContext.FilterHistories.Update(obj);
                    await dBContext.SaveChangesAsync();
                }

                return "Deleted";

            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        private void MapProperties(object source, object destination)
        {
            var sourceProperties = source.GetType().GetProperties();
            var destinationProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);

                if (destinationProperty != null && destinationProperty.CanWrite)
                {
                    destinationProperty.SetValue(destination, sourceProperty.GetValue(source));
                }
            }
        }

        public async Task<string> GetLatestMUFNo()
        {
            try
            {
                long MaxId = 0;
                var maxMUF = await dBContext.Lnedetails.OrderByDescending(e => e.Id).FirstOrDefaultAsync();
                if (maxMUF != null)
                {
                    MaxId = maxMUF.Id;
                }
                return Convert.ToString(MaxId + 1);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public async Task<Dictionary<string, object>> GetShiftDayWeek(string StartDateTime)
        {
            try
            {
                Dictionary<string, object> response = new Dictionary<string, object>();
                DateTime? _dateandTime = null;
                if (!string.IsNullOrEmpty(StartDateTime))
                {
                    if (DateTime.TryParseExact(StartDateTime.Trim(), "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        _dateandTime = result;
                    }
                    else if (DateTime.TryParseExact(StartDateTime.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result2))
                    {
                        _dateandTime = result2;
                    }

                }
                if (_dateandTime != null)
                {
                    var employeeObject = await dBContext.Weeks.Where(x => (_dateandTime.Value.Date >= x.StartDate.Value.Date && _dateandTime.Value.Date <= x.EndDate.Value.Date)).FirstOrDefaultAsync();
                    if (employeeObject != null)
                        response.Add("WeekNumber", employeeObject.Title);

                    var shiftNumberObject = await dBContext.ShiftNumbers.Where(x => (_dateandTime.Value >= x.StartShiftDateTime && _dateandTime.Value <= x.EndShiftDateTime)).FirstOrDefaultAsync();
                    if (shiftNumberObject != null)
                        response.Add("ShiftNumber", shiftNumberObject.ShiftNo);
                }

                return response;

            }
            catch (Exception ex)
            {
                return null;

            }
        }
    }

}
