using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System.Data;
using PRISM.DTO;
using System.ComponentModel;
using OfficeOpenXml;
using System.Reflection;
using PRISM.DTO.MainShift;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Security.Claims;
using PRISM.DTO.WeeklyCommentsdto;
using System.Globalization;

namespace PRISM.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeServices _homeServices;
        private readonly IEmployees _employeeServices;
        private readonly IDistributions distributionServices;
        private readonly IContacts _contacts;
        private readonly IWebHostEnvironment environment;
        private readonly ILookupServices _lookup;
        private readonly IMachines _machines;
        private readonly IRoutes _locations;
        private readonly IConfiguration configuration;
        public HomeController(ILogger<HomeController> logger, IHomeServices homeServices, IEmployees employees, IContacts contacts
            , IWebHostEnvironment _environment, IConfiguration _configuration, ILookupServices lookup, IMachines machines, IDistributions distributionsServices
            , IRoutes locations)
        {
            _logger = logger;
            _homeServices = homeServices;
            _employeeServices = employees;
            _contacts = contacts;
            environment = _environment;
            configuration = _configuration;
            _lookup = lookup;
            _machines = machines;
            distributionServices = distributionsServices;
            _locations = locations;
        }
        static string GetClaimValue(ClaimsIdentity identity, string claimType)
        {
            Claim claim = identity?.FindFirst(claimType);
            return claim?.Value;
        }
        public async Task<IActionResult> Index()
        {
            var identity = User.Identity as ClaimsIdentity;

            // Get the user's object identifier (OID) from the ClaimsPrincipal
            var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			Claim claim = identity?.Claims.FirstOrDefault(x => x.Type == "name");
			string fullName = claim?.Value;
			
			string email = GetClaimValue(identity, ClaimTypes.Email);
            var userInfo = new AppUser()
            {
                RecordStatus = "Active",
                EmailAddress = email,
                FullName = fullName,
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                RoleId = 0
            };

            var user = await _employeeServices.InsertAppUser(userInfo);

            if (identity != null)
            {
                try
                {
                    var rolesModeuls = await _employeeServices.GetRolesAndRightsByUserId(userId);
                    HttpContext.Session.SetString("UserRoles", JsonConvert.SerializeObject(rolesModeuls));
                }
                catch (Exception ex)
                {
                }
               
            }

            List<string> typeLookup = new List<string>();
            typeLookup.Add("Statuses");
            typeLookup.Add("VSTPContact");
            typeLookup.Add("ACT");
            typeLookup.Add("ShiftStatus");
            typeLookup.Add("Organisations");
            typeLookup.Add("ChangePeriod");
            typeLookup.Add("ChangeType");
            typeLookup.Add("FurtherAction");
            typeLookup.Add("MachineDepartment");
            HomeFilterModel objModel = new HomeFilterModel();
            objModel.WeekList = await _lookup.GetWeeks();
            objModel.MachinesList = await _machines.GetMachines();
            objModel.StatusList = await _lookup.GetLookups(typeLookup);
            objModel.TemplateList = await _lookup.GetTemplates();
            objModel.FilterHistory = await _homeServices.GetFilterHistory(userId);
            objModel.LocationList = await _locations.GetData();
            EmployeeModel model = new EmployeeModel();
            model.EmployeeList = await _employeeServices.GetData(0);
            model.LeaveTypeList = await _lookup.GetLeaves();
            // model.AbsanceList = await _absances.GetData();
            model.type = 0;
           

            objModel.AbsanceModel = model;
            return View(objModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("Home/GetData")]
        public async Task<IActionResult> GetData([FromQuery] SearchShiftModel param)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Use the userId as needed

                Dictionary<string, object> response = new Dictionary<string, object>();
                var obj = await _homeServices.GetData(param, userId);
                List<string> strings = new List<string>();
                var coontactlist = await _contacts.GetData("");
                var employees = await _employeeServices.GetData(0);
                var weeklyComments = await _homeServices.GetWeeklyComments(Convert.ToInt32(string.IsNullOrEmpty(param.FromWeek) ? "0" : param.FromWeek));
                var DistributionList = await distributionServices.GetData("home");
                response.Add("ShiftData", obj);
                response.Add("ContactData", coontactlist);
                response.Add("EmployeeData", employees);
                response.Add("WeeklyComments", weeklyComments);
                response.Add("DistributionList", DistributionList);

                PropertyInfo[] ClassProperties;
                ClassProperties = typeof(LNEDetailModel).GetProperties();
                List<string> colNames = new List<string>();
                foreach (PropertyInfo propertyInfo in ClassProperties)
                {
                    colNames.Add(propertyInfo.Name);
                }
                response.Add("ColumnNames", colNames);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("Home/GetExcelExport")]
        public async Task<JsonResult> GetExcelExport([FromQuery] SearchShiftModel param)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.GetData(param, userId);
                string _RootPath = System.IO.Path.Combine(this.environment.WebRootPath, "ShiftLists");
                string _folderPath = System.IO.Path.Combine(_RootPath, "excelimport.xlsx");
                ExportToExcel(obj, _folderPath);
              
                return Json(Ok("excelimport.xlsx"));
            }
            catch (Exception ex)
            {
                return Json(BadRequest(ex.Message));
            }

        }
        public void ExportToExcel<T>(List<T> dataList, string filePath)
        {
            if (dataList == null || !dataList.Any())
                return;

            // Get properties of the object type
            PropertyInfo[] properties = typeof(T).GetProperties();

            // Extract property names
            string[] headers = properties.Select(p => p.Name).ToArray();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            // Create a new Excel package
            using (var excelPackage = new ExcelPackage())
            {
                // Add a new worksheet to the Excel package
                var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                // Add headers to the worksheet
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Populate data rows
                for (int i = 0; i < dataList.Count; i++)
                {
                    var dataItem = dataList[i];
                    for (int j = 0; j < properties.Length; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = properties[j].GetValue(dataItem);
                    }
                }

                // Save the Excel package to a file
                System.IO.File.WriteAllBytes(filePath, excelPackage.GetAsByteArray());
            }
        }


        [HttpGet]
        [Route("Home/GetDataById")]
        public async Task<IActionResult> GetDataById(int id)
        {
            try
            {
                var response = await _homeServices.GetDataById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("Home/GetShiftDataById")]
        public async Task<IActionResult> GetShiftDataById(int shiftId)
        {
            try
            {
                var response = await _homeServices.GetShiftDataById(shiftId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("Home/GetLogTypes")]
        public async Task<IActionResult> GetLogTypes(string type, int shiftid)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.GetLogTypes(type, shiftid, userId);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/insert")]
        public async Task<JsonResult> insert([FromBody] LNEDetailModel model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.Insert(model, userId);
                return Json(Ok(obj));
            }
            catch (Exception ex)
            {
                return Json(BadRequest());
            }

        }
        [HttpPost]
        [Route("Home/insertGridMileStone")]
        public async Task<IActionResult> insertGridMileStone([FromBody] MileStoneEntry model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.InsertGridMileStone(model, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/insertPersons")]
        public async Task<IActionResult> insertPersons([FromBody] ShiftDetailEmployeeModel model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.InsertPersons(model, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/CheckIfExist")]
        public async Task<IActionResult> CheckIfExist([FromBody] ShiftDetailEmployeeModel model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.CheckIfExist(model, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/insertShiftContact")]
        public async Task<IActionResult> insertShiftContact([FromBody] ShiftContact model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;


                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.InsertShiftContact(model, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/deleteGridMileStone")]
        public async Task<IActionResult> deleteGridMileStone([FromBody] CommonId model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.deleteGridMileStone(model.Id, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }
        [HttpPost]
        [Route("Home/deletePersons")]
        public async Task<IActionResult> deletePersons([FromBody] CommonId model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.deletePersons(model.Id, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }
        [HttpPost]
        [Route("Home/deleteShiftContact")]
        public async Task<IActionResult> deleteShiftContact([FromBody] CommonId model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _homeServices.deleteShiftContact(model.Id, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }
        [HttpPost]
        public async Task<IActionResult> ImportFile()
        {
            try
            {
                var httpRequest = Request;

                var resposne = new Dictionary<string, object>();

                string _RootPath = System.IO.Path.Combine(this.environment.WebRootPath, "TempExcell");
                List<ColumnDetailImport> _ColumnList = new List<ColumnDetailImport>();
                List<ColumnDetailImport> _deserealizeObj = new List<ColumnDetailImport>();
                if (httpRequest.Form.Files.Count > 0)
                {


                    string _folderPath = System.IO.Path.Combine(_RootPath, "excelimport.xls");
                    if (!System.IO.Directory.Exists(_RootPath))
                    {
                        System.IO.Directory.CreateDirectory(_RootPath);
                    }
                    string filePath = System.IO.Path.Combine(_RootPath, "excelimport.xls");
                    //  databasepath = System.IO.Path.Combine(databasepath, param.Id.ToString() + "_" + fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await httpRequest.Form.Files[0].CopyToAsync(stream);
                    }
                    DataTable dtCsv = new DataTable();
                    string Fulltext;

                    // string filePath = _folderPath;
                    FileStream fs = new FileStream(_folderPath, FileMode.Open, FileAccess.Read);
                    HSSFWorkbook workbook = new HSSFWorkbook(fs);

                    // Assuming you want to read the first sheet
                    ISheet sheet = workbook.GetSheetAt(3);

                    // Iterate over the rows
                    for (int row = 0; row <= 0; row++)
                    {

                        IRow currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        if (row == 0)
                        {
                            for (int c = 0; c < currentRow.LastCellNum; c++)
                            {
                                dtCsv.Columns.Add(currentRow.GetCell(c)?.ToString() ?? string.Empty);
                            }
                        }
                        else
                        {


                            //DataRow dr = dtCsv.NewRow();
                            //bool IsRoadHasData = false;
                            //for (int c = 1; c <= currentRow.LastCellNum; c++)
                            //{
                            //    if (!string.IsNullOrEmpty(Convert.ToString(currentRow.GetCell(c)?.ToString() ?? string.Empty)))
                            //        IsRoadHasData = true;

                            //    dr[c - 1] = currentRow.GetCell(c)?.ToString() ?? string.Empty;
                            //}
                            //if (IsRoadHasData)
                            //    dtCsv.Rows.Add(dr);

                        }



                        // Iterate over the cells
                        //for (int col = 0; col < currentRow.LastCellNum; col++)
                        //{
                        //    string cellValue = currentRow.GetCell(col)?.ToString() ?? string.Empty;
                        //    Console.Write(cellValue + "\t");
                        //}
                        //Console.WriteLine();
                    }



                    //rowIndex = 2;

                    //for (int r = rowIndex; r <= noOfRow; r++)
                    //{
                    //    DataRow dr = dtCsv.NewRow();
                    //    bool IsRoadHasData = false;
                    //    for (int c = 1; c <= noOfCol; c++)
                    //    {
                    //        if (!string.IsNullOrEmpty(Convert.ToString(workSheet.Cells[r, c].Value)))
                    //            IsRoadHasData = true;

                    //        dr[c - 1] = workSheet.Cells[r, c].Value;
                    //    }
                    //    if(IsRoadHasData)
                    //        dtCsv.Rows.Add(dr);
                    //}


                    //using (ExcelPackage excelPackage = new ExcelPackage(_folderPath))
                    //{
                    //    //ExcelWorksheet ws = excelPackage.Workbook.Worksheets.First();
                    //    //ws.Cells[1, 1].Value = "Test1";
                    //    //excelPackage.Save();


                    //    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    //    ExcelWorksheets workSheets = excelPackage.Workbook.Worksheets;
                    //    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];

                    //    int noOfCol = workSheet.Dimension.End.Column;
                    //    int noOfRow = workSheet.Dimension.End.Row;
                    //    int rowIndex = 1;

                    //    for (int c = 1; c <= noOfCol; c++)
                    //    {
                    //        dtCsv.Columns.Add(workSheet.Cells[rowIndex, c].Text);
                    //    }
                    //    //rowIndex = 2;

                    //    //for (int r = rowIndex; r <= noOfRow; r++)
                    //    //{
                    //    //    DataRow dr = dtCsv.NewRow();
                    //    //    bool IsRoadHasData = false;
                    //    //    for (int c = 1; c <= noOfCol; c++)
                    //    //    {
                    //    //        if (!string.IsNullOrEmpty(Convert.ToString(workSheet.Cells[r, c].Value)))
                    //    //            IsRoadHasData = true;

                    //    //        dr[c - 1] = workSheet.Cells[r, c].Value;
                    //    //    }
                    //    //    if(IsRoadHasData)
                    //    //        dtCsv.Rows.Add(dr);
                    //    //}

                    //}



                    int count = 1;
                    foreach (DataColumn column in dtCsv.Columns)
                    {

                        _ColumnList.Add(new ColumnDetailImport()
                        {
                            Id = count,
                            FilesColName = column.ColumnName.Replace(" ", "")//string.IsNullOrEmpty(fileColumn)? column.ColumnName: fileColumn,
                                                                             //DBColName =dbColumn,
                        });
                        count++;
                    }


                    // System.IO.File.Delete(_folderPath);
                    resposne.Add("FileColumnList", _ColumnList);

                    return Ok(resposne);
                }
                else
                {

                    return Ok(resposne);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<JsonResult> SaveImportData([FromBody] ImportDataModel param)
        {
            try
            {

                string result = "success";

                DataTable dtCsv = new DataTable();
                string _RootPath = System.IO.Path.Combine(this.environment.WebRootPath, "TempExcell");
                string _folderPath = System.IO.Path.Combine(_RootPath, "excelimport.xls");
                FileStream fs = new FileStream(_folderPath, FileMode.Open, FileAccess.Read);
                HSSFWorkbook workbook = new HSSFWorkbook(fs);

                // Assuming you want to read the first sheet
                ISheet sheet = workbook.GetSheetAt(3);

                // Iterate over the rows
                for (int row = 0; row <= sheet.LastRowNum; row++)
                {

                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    if (row == 0)
                    {
                        for (int c = 0; c < currentRow.LastCellNum; c++)
                        {
                            dtCsv.Columns.Add(Convert.ToString(currentRow.GetCell(c)).Replace(" ", "") ?? string.Empty);
                        }
                    }
                    else
                    {


                        DataRow dr = dtCsv.NewRow();
                        bool IsRoadHasData = false;
                        for (int c = 0; c < currentRow.LastCellNum; c++)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(currentRow.GetCell(c)?.ToString() ?? string.Empty)))
                                IsRoadHasData = true;

                            dr[c] = currentRow.GetCell(c)?.ToString() ?? string.Empty;
                        }
                        if (IsRoadHasData)
                            dtCsv.Rows.Add(dr);

                    }



                    // Iterate over the cells
                    //for (int col = 0; col < currentRow.LastCellNum; col++)
                    //{
                    //    string cellValue = currentRow.GetCell(col)?.ToString() ?? string.Empty;
                    //    Console.Write(cellValue + "\t");
                    //}
                    //Console.WriteLine();
                }


                List<Lnedetail> listme = new List<Lnedetail>();
                foreach (DataRow csvr in dtCsv.Rows)
                {

                    var colName = new List<string>();
                    var InsertParameters = new Dictionary<string, object>();
                    int FileIndex = 0;
                    Lnedetail obj = new Lnedetail();
                    PropertyInfo[] properties = typeof(Lnedetail).GetProperties();
                    foreach (var cols in param.DBColName)
                    {
                        if (param.FilesColName.Count > FileIndex)
                        {
                            if (!string.IsNullOrEmpty(cols) && cols != "Id")
                            {
                                int colCount = 0;
                                string valuesDB = "";
                                List<string> col = new List<string>();
                                if (!string.IsNullOrEmpty(param.FilesColName[FileIndex]))
                                {
                                    valuesDB = Convert.ToString(csvr[param.FilesColName[FileIndex]]);
                                }

                                if (!string.IsNullOrEmpty(param.FilesColName[FileIndex]))
                                {
                                    //InsertParameters.Add(cols, valuesDB);


                                    var onobj = properties.Where(x => x.Name.ToLower() == cols.ToLower()).FirstOrDefault();
                                    if (onobj != null)
                                    {
                                        if (onobj.PropertyType == typeof(string))
                                        {
                                            onobj.SetValue(obj, valuesDB);
                                        }
                                        else if (onobj.PropertyType == typeof(DateTime?) || onobj.PropertyType == typeof(DateTime))
                                        {
											onobj.SetValue(obj, string.IsNullOrEmpty(valuesDB) ? null : Convert.ToDateTime(valuesDB, CultureInfo.InvariantCulture));
											//string[] formats = { "MM/dd/yyyy", "M/d/yy", "M/d/yyyy" };

											//// Attempt to parse the datetime using the defined formats
											//if (string.IsNullOrEmpty(valuesDB))
											//{
											//    onobj.SetValue(obj, null);
											//}
											//DateTime resultDatetime;
											//if (DateTime.TryParseExact(valuesDB, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultDatetime))
											//{
											//    onobj.SetValue(obj, resultDatetime);
											//}
											//else
											//{
											//    onobj.SetValue(obj, null);
											//}

										}
                                    }
                                    //foreach (PropertyInfo property in properties)
                                    //{
                                    //    if (property.Name.ToLower() == cols.ToLower())
                                    //    {
                                    //        var value = valuesDB;
                                    //        property.SetValue(obj, value);
                                    //        break;
                                    //    }
                                    //}
                                    //colValue.Add(valuesDB);//colValue = colValue + ",'" + Convert.ToString(r[cols.FilesColName]) + "'";
                                }





                            }

                            FileIndex++;
                        }
                    }
                    listme.Add(obj);

                }

                var identity = User.Identity as ClaimsIdentity;

                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                result = await _homeServices.InsertBulkData(listme, userId);

                // }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        [HttpGet]
        [Route("Home/GetDriverQuestion")]
        public async Task<IActionResult> GetDriverQuestion([FromQuery] int id, int driverId)
        {
            try
            {
                var response = await _homeServices.GetDriverQuestion(id, driverId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/SaveMedication")]
        public async Task<IActionResult> SaveMedication([FromBody] DriverQuestion model)
        {
            try
            {
                var obj = await _homeServices.SaveDriverQuestion(model);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }

        [HttpPost]
        [Route("Home/InsertVSTP")]
        public async Task<IActionResult> InsertVSTP([FromBody] MachineShiftVstp model)
        {
            try
            {
                var obj = await _homeServices.InsertVSTP(model);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/SaveShiftStatus")]
        public async Task<IActionResult> SaveShiftStatus([FromBody] ShiftStatusModel model)
        {
            try
            {
                var obj = await _homeServices.SaveShiftStatus(model.ShiftStatus, model.Ids);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/DuplicateRecords")]
        public async Task<IActionResult> DuplicateRecords([FromBody] ShiftStatusModel model)
        {
            try
            {
                var obj = await _homeServices.DuplicateRecords(model.Ids, model.IsHoursAdded);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/SaveWeeklyComments")]
        public async Task<IActionResult> SaveWeeklyComments([FromBody] WeeklyCommentsModel model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                model.UserId = userId;
                var obj = await _homeServices.SaveWeeklyComments(model);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("Home/ResetFilters")]
        public async Task<IActionResult> ResetFilters()
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var obj = await _homeServices.ResetFilters(userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpGet]
        [Route("Home/GetLatestMUFNo")]
        public async Task<IActionResult> GetLatestMUFNo()
        {
            try
            {
                var response = await _homeServices.GetLatestMUFNo();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("Home/GetChangeLog")]
        public async Task<IActionResult> GetChangeLog(int ShiftId)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var UserLog = await _employeeServices.GetUserLog(userId, ShiftId);
                // var response = await _homeServices.GetChangeLog();
                return Ok(UserLog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
       
        [HttpGet]
        [Route("Home/GetShiftDayWeek")]
        public async Task<IActionResult> GetShiftDayWeek(string DateTimeMix)
        {
            try
            {
                // var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                //var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var UserLog = await _homeServices.GetShiftDayWeek(DateTimeMix);
                // var response = await _homeServices.GetChangeLog();
                return Ok(UserLog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
