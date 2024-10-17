using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using OfficeOpenXml;
using PRISM.DTO;
using PRISM.DTO.ChangeLogDTO;
using PRISM.DTO.ReportsModels;
using PRISM.Models;
using PRISM.Services;
using PRISM.Services.Interfaces;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Web.Http.Results;

namespace PRISM.Controllers
{
    public class ChangeLogController : Controller
	{
		private readonly IContacts _contacts;
		private readonly IChangeLogServices _changelogservices;
        private readonly ILookupServices _lookup;
        private readonly IWebHostEnvironment environment;
		private readonly IEmployees _employeeServices;
		public ChangeLogController(IWebHostEnvironment _environment,ILookupServices lookupServices, IChangeLogServices changeLogServices, IEmployees employees)
		{
            _changelogservices = changeLogServices;
			environment = _environment;
			this._lookup = lookupServices;
			_employeeServices = employees;
		}
		
		[HttpPost]
		[Route("ChangeLog/insert")]
		public async Task<JsonResult> insert([FromBody] ChangeLogModel model)
		{
			try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var obj =await _changelogservices.Insert(model, userId);
				return Json(Ok(obj));
			}
			catch (Exception ex)
			{
				return Json(BadRequest(ex.Message));
			}

		}
        [HttpGet]
        [Route("ChangeLog/GetChangeLog")]
        public async Task<IActionResult> GetChangeLog(int ShiftId)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var UserLog = await _changelogservices.GetChangeLog(userId, ShiftId);
                // var response = await _homeServices.GetChangeLog();
                return Ok(UserLog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("ChangeLog/GetChangeLogByDate")]
        public async Task<IActionResult> GetChangeLogByDate(int FromWeek,int ToWeek)
        {
            try
            {
                var UserLog = await _changelogservices.GetChangeLogByDate(FromWeek, ToWeek);
                return Ok(UserLog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("ChangeLog/ExportExcel")]
        public async Task<JsonResult> ExportExcel(int ShiftId)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _changelogservices.GetChangeLog(userId, ShiftId);
                string _RootPath = System.IO.Path.Combine(this.environment.WebRootPath, "ShiftLists");
                string _folderPath = System.IO.Path.Combine(_RootPath, "excelChangeLog.xlsx");
                ExportToExcel(obj, _folderPath);

                return Json(Ok("excelChangeLog.xlsx"));
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
		[Route("ChangeLog/GetUserLog")]
		public async Task<IActionResult> GetUserLog(int UserIdFrom)
		{
			try
			{
				var identity = User.Identity as ClaimsIdentity;

				// Get the user's object identifier (OID) from the ClaimsPrincipal
				var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var UserLog = await _employeeServices.GetUserLog(userId, UserIdFrom);
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
