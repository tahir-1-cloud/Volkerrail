using DinkToPdf;
using DinkToPdf.Contracts;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.CallRecords;
using Microsoft.Graph.SecurityNamespace;
using Nancy;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using NuGet.ContentModel;
using OfficeOpenXml;
using PRISM.DTO;
using PRISM.DTO.ReportsModels;
using PRISM.EventHandlersBLL;
using PRISM.Models;
using PRISM.Services;
using PRISM.Services.Interfaces;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;
using static iText.Kernel.Pdf.Colorspace.PdfPattern;
using static NPOI.HSSF.Util.HSSFColor;
using static System.Reflection.Metadata.BlobBuilder;

namespace PRISM.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IContacts _contacts;
        private readonly PRISMContext _dbContext;
        private readonly IHomeServices _homeServices;
        private readonly IMachines _machines;
        private readonly IReportServices _reportServices;
        private readonly ILookupServices _lookup;
        private readonly IWebHostEnvironment environment;
        private readonly IDistributions distributionServices;
        private readonly IConverter _converter;
        public ReportsController(IContacts contacts, PRISMContext dbContext, IHomeServices homeServices, IMachines machines, IReportServices reportServices
            , IWebHostEnvironment _environment, ILookupServices lookupServices, IDistributions distributions)
        {
            this._contacts = contacts;
            this._dbContext = dbContext;
            this._homeServices = homeServices;
            this._machines = machines;
            _reportServices = reportServices;
            environment = _environment;
            this._lookup = lookupServices;
            this.distributionServices = distributions;
            //_converter = converter;
        }
        [Route("Reports")]
        [Route("Reports/index")]
        public async Task<IActionResult> Index(string Type, int ShiftId, int FromWeek, int ToWeek, string Dep = "")
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                RootReportModel resultReport = new RootReportModel();


                if (Type == "VSTP")
                {

                    VSTPViewModel vSTPViewModel = new VSTPViewModel();
                    vSTPViewModel.LNEDetailModel = new DTO.MainShift.LNEDetailModel();

                    vSTPViewModel.LNEDetailModel = await _homeServices.GetShiftDataById(ShiftId);
                    if (vSTPViewModel.LNEDetailModel != null)
                    {
                        vSTPViewModel.MachineModel = new Models.Machine();

                        vSTPViewModel.MachineModel = await _dbContext.Machines.Where(x => x.Number == vSTPViewModel.LNEDetailModel.MachineNum).FirstOrDefaultAsync();
                    }
                    vSTPViewModel.MachineShiftVstpModel = new MachineShiftVstp();

                    vSTPViewModel.MachineShiftVstpModel = await _dbContext.MachineShiftVstps.Where(x => x.ShiftId == ShiftId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    resultReport.vSTPViewModel = new VSTPViewModel();
                    resultReport.vSTPViewModel = vSTPViewModel;
                    resultReport.WeekList = new List<Week>();
                    resultReport.WeekList = await _lookup.GetWeeks();
                    return View(resultReport);
                }
                else if (Type == "PLANTCREWROSTERREPORT")
                {
                    resultReport = await _reportServices.GetWeeklyRosterReport(FromWeek, ToWeek, userId);
                    resultReport.WeekList = await _lookup.GetWeeks();
                    return View(resultReport);
                }
                else if (Type == "CONDUCTORREPORT")
                {
                    resultReport = await _reportServices.GetConductionChronReport(FromWeek, ToWeek, userId);
                    resultReport.WeekList = await _lookup.GetWeeks();
                    return View(resultReport);
                }
                else if (Type == "INTERNALCOMMENTREPORT")
                {
                    resultReport = await _reportServices.GetInternalCommentsReport(FromWeek, ToWeek, userId);
                    resultReport.WeekList = await _lookup.GetWeeks();
                    return View(resultReport);
                }
                else if (Type == "BOXREPORT")
                {
                    List<string> typeLookup = new List<string>();
                    typeLookup.Add("MachineDepartment");

                    resultReport = await _reportServices.GetBoxReport(FromWeek, ToWeek, userId, Dep);
                    resultReport.WeekList = await _lookup.GetWeeks();
                    resultReport.LookupList = await _lookup.GetLookups(typeLookup);

                    return View(resultReport);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        [HttpPost]
        [Route("Reports/insert")]
        public async Task<IActionResult> insert([FromBody] Models.Contact model)
        {
            try
            {
                var obj = await _contacts.Insert(model);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Reports/delete")]
        public async Task<IActionResult> delete([FromBody] CommonId model)
        {
            try
            {
                var obj = await _contacts.delete(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
        [HttpPost]
        [Route("Reports/GenerateReport")]
        public JsonResult GenerateReport([FromBody] PDFModelGenerate param)
        {
            try
            {
                // string htmlContent = "<h1>Hello, PDF!</h1>";

                ////// Generate PDF from HTML

                //byte[] pdfBytes = _reportServices.GeneratePdf(param.ReportHTML);

                // // Return the PDF as a file
                // return File(pdfBytes, "application/pdf", param.ReportType + ".pdf");
                string filePathDate = GenerateUniqueString();

                string _RootPath = System.IO.Path.Combine(this.environment.WebRootPath, "ReportsPDF");
                string FilePath = System.IO.Path.Combine(_RootPath, filePathDate + param.ReportType + "Report.pdf");

                if (System.IO.File.Exists(FilePath))
                {
                    // Delete the file
                    System.IO.File.Delete(FilePath);
                }

                PdfWriter pdfWriter = new PdfWriter(FilePath);

                iText.Html2pdf.ConverterProperties properties = new iText.Html2pdf.ConverterProperties();

                //old code
                iText.Layout.Font.FontProvider fontProvider = new iText.Html2pdf.Resolver.Font.DefaultFontProvider();
                properties.SetFontProvider(fontProvider);

                properties.SetFontProvider(fontProvider);

                //new code

                iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter);

                if (param.ReportType == "BOXREPORT" || param.ReportType == "VSTP")
                {
                    var pageSize = new iText.Kernel.Geom.PageSize(1480, 2000);
                    pageSize = (iText.Kernel.Geom.PageSize)pageSize.ApplyMargins(0, 0, 0, 0, false);
                    if (param.ReportType == "VSTP")
                    {
                        pdfDocument.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4);
                    }
                    else if (param.ReportType == "BOXREPORT")
                    {
                        pageSize = new iText.Kernel.Geom.PageSize(1450, 800);
                        pageSize = (iText.Kernel.Geom.PageSize)pageSize.ApplyMargins(0, 0, 0, 0, false);
                        pdfDocument.SetDefaultPageSize(pageSize);
                    }
                    else
                        pdfDocument.SetDefaultPageSize(pageSize);
                }
                else
                {
                    var pageSize = new iText.Kernel.Geom.PageSize(792, 612);
                    pageSize = (iText.Kernel.Geom.PageSize)pageSize.ApplyMargins(0, 0, 0, 0, false);

                    pdfDocument.SetDefaultPageSize(pageSize);

                }


                iText.Layout.Document document = iText.Html2pdf.HtmlConverter.ConvertToDocument(param.ReportHTML, pdfDocument, properties);

                document.Close();

                return Json(Ok(filePathDate + param.ReportType + "Report.pdf"));
                //var obj = await _contacts.delete(model.Id);
            }
            catch (Exception ex)
            {
                return Json(BadRequest(ex.Message));
            }


        }

        [HttpPost]
        [Route("Reports/GenerateBOXReport")]
        public async Task<JsonResult> GenerateBOXReport([FromBody] PDFModelGenerate param)
        {
            try
            {

                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var _boxReportModel = await _reportServices.GetBoxReport(param.FromWeek, 0, userId, param.Department);


                BoxReportModel lastBoxReport = _boxReportModel.boxReportModel
      .SelectMany(machineDepartmentsModel => machineDepartmentsModel.boxReportModel)
      .SelectMany(machineBoxReportModel => machineBoxReportModel.boxReportModels)
      .LastOrDefault();

                string filePathDate = GenerateUniqueString();

                string _RootPath = System.IO.Path.Combine(this.environment.WebRootPath, "ReportsPDF");
                string FilePath = System.IO.Path.Combine(_RootPath, filePathDate + param.ReportType + "Report.pdf");

                if (System.IO.File.Exists(FilePath))
                {
                    // Delete the file
                    System.IO.File.Delete(FilePath);
                }

                PdfWriter pdfWriter = new PdfWriter(FilePath);
                pdfWriter.SetCloseStream(false);
                // Text.Layout.Font.FontProvider fontProvider = new iText.Html2pdf.Resolver.Font.();
                //string arialFontPath =System.IO.Path.Combine(this.environment.WebRootPath, "font", "Arial.ttf");
                //   FontProvider fontProvider = new DefaultFontProvider();
                //FontProvider fontProvider = new FontProvider();
                // fontProvider.AddFont(fontProvider);
                FontProvider fontProvider = new FontProvider();
                fontProvider.AddFont("Helvetica");
                iText.Html2pdf.ConverterProperties properties = new iText.Html2pdf.ConverterProperties();
                properties.SetFontProvider(fontProvider);
                //properties.SetFontProvider(fontProvider);

                iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter);
                pdfDocument.SetCloseWriter(false);
                var pageSize = new iText.Kernel.Geom.PageSize(1450, 825);
                pageSize = (iText.Kernel.Geom.PageSize)pageSize.ApplyMargins(0, 0, 0, 0, false);
                pdfDocument.SetDefaultPageSize(pageSize);



                string html = $@"<table cellspacing='0' cellpadding='0' style='border: 0pt solid rgb(0, 0, 0); border-collapse: collapse;width:100%'>
                      <tr font-family: 'Helvetica'>
                          <td>
                              <p style='font-size:16px;text-align:center'>
                                  <span style='font-family:Helvetica'><b>WEEK NO:  {param.WeekNumber} </b></span>
                              </p>
                          <td>
                          <td>
                              <p style='font-size:16px;text-align:center'>
                                  <span style='font-family:Helvetica'><b>  {(DateTime.Now.ToString("dd-MM-yyyy"))} </b></span>
                              </p>
                          </td>

                          <td style='text-align:center'>
                              <h4 style='color:darkblue;margin-bottom:1px'> Commencing Saturday {param.WeekDateTime}</h4>


                          </td>

                          <td>
                              <p style='font-size:16px;text-align:center'>
                                  <span style='font-family:Helvetica'><img src='https://devprism.azurewebsites.net/assets/media/logos/pdfreportlogo.PNG' style='width:150px' /></span>
                              </p>
                          </td>
                      </tr>
                  </table>";



                //old code

                //if (_boxReportModel.boxReportModel?.Count > 0)
                //{
                //    int counter = 1;
                //    int depcount = 0;
                //    foreach (var department in _boxReportModel.boxReportModel)
                //    {
                //        depcount = 0;



                //        if (department.boxReportModel?.Count > 0)
                //        {


                //            foreach (var machine in department.boxReportModel)
                //            {
                //                if (machine.IsShiftExist)
                //                {
                //                    if (depcount == 0)
                //                        html += $@"<div><b>{department.DepartmentName} </b></div>";

                //                    depcount++;
                //                    html += $@"<table style='page-break-inside: avoid;'><tr>
                //                                  <td colspan='{machine.boxReportModels?.Count}'>{machine.MachineNumber}</td></tr><tr>";
                //                    long shiftIdlast = 0;
                //                    foreach (var item in machine.boxReportModels)
                //                    {
                //                        shiftIdlast = item.Id;
                //                        if (item.Id == 0)
                //                        {
                //                            html += $@"<td style='width: 120px;height: 140px;vertical-align:top;
                //border: 1px solid black;
                //margin-top: 0px;
                //font-size: 10px;
                //padding: 3px;
                //line-height: 12px;
                //font-family: 'Times New Roman';'>
                //                                            <table style='width:100%'>
                //                                                <tr>
                //                                                    <td style='border-bottom:.5px solid black'>{item.RowNum}</td>
                //                                                    <td style='border-bottom:.5px solid black'></td>
                //                                                    <td style='border-bottom:.5px solid black'></td>
                //                                                </tr>
                //                                            </table>
                //                                        </td>";
                //                        }
                //                        else
                //                        {
                //                            html += $@"<td style='
                //                                                  width: 120px;height: 140px;vertical-align:top;
                //  border: 1px solid black;
                //  margin-top: 0px;
                //  font-size: 10px;
                //  padding: 3px;
                //  background-color:{(item.WorksiteDetails == "Workshops" ? "white" : "rgb(192, 255, 192)")}; 
                //  line -height: 12px;
                //  font-family: 'Times New Roman';
                //          '>
                //                                              <table style=""width:100%"">
                //                                                  <tr>
                //                                                      <td>#{item.RowNum}</td>
                //                                                      <td>{item.PpreDay}</td>
                //                                                      <td>{item.StartDateTime}</td>
                //                                                  </tr>

                //                                                  <tr>
                //                                                      <td colspan=""3"">{item.WorksiteDetails}</td>

                //                                                  </tr>
                //                                                  <tr>
                //                                                      <td colspan=""3"">{item.WorkDescription}</td>

                //                                                  </tr>
                //                                                  <tr>
                //                                                      <td colspan=""3"">{(item.StartTime + "-" + item.FinishTime + "(" + item.CalculatedTime + ")")}</td>

                //                                                  </tr>
                //                                                  <tr>
                //                                                      <td colspan=""3""><span>{item.OwnerName}</span> <span>{item.WorksiteELR}</span></td>

                //                                                  </tr>
                //                                                  <tr>
                //                                                      <td colspan=""3"">{item.PpreDriver}</td>

                //                                                  </tr>
                //                                                  <tr>
                //                                                      <td colspan=""3"">{item.Conductor}</td>

                //                                                  </tr>
                //                                                  <tr>
                //                                                      <td colspan=""3"">{(item.InShortCode + "   " + item.OutShortCode + "    " + item.Id.ToString())}</td>

                //                                                  </tr>
                //                                                  <tr>
                //                                                      <td colspan=""3"" style=""text-align:right"">{item.PTONumber}</td>

                //                                                  </tr>
                //                                              </table>
                //                                          </td>";
                //                        }


                //                    }
                //                    html += "</tr></table>";

                //                    if (counter % 5 == 0)
                //                    {

                //                        html += $@"<table cellspacing='0' cellpadding='0' style='border: 0pt solid rgb(0, 0, 0); border-collapse: collapse;width:100%;page-break-before: always;'>
                //          <tr>
                //              <td>
                //                  <p style='font-size:14px;text-align:center'>
                //                      <span style='font-family:Calibri'><b>WEEK NO:  {param.WeekNumber} </b></span>
                //                  </p>
                //              <td>
                //              <td>
                //                  <p style='font-size:14px;text-align:center'>
                //                      <span style='font-family:Calibri'><b>  {(DateTime.Now.ToString("dd-MM-yyyy"))} </b></span>
                //                  </p>
                //              </td>

                //              <td style='text-align:center'>
                //                  <h4 style='color:darkblue;margin-bottom:1px'> Commencing Saturday {param.WeekDateTime}</h4>


                //              </td>

                //              <td>
                //                  <p style='font-size:14px;text-align:center'>
                //                      <span style='font-family:Calibri'><img src='https://devprism.azurewebsites.net/assets/media/logos/pdfreportlogo.PNG' style='width:150px' /></span>
                //                  </p>
                //              </td>
                //          </tr>
                //      </table>";
                //                    }
                //                    counter++;
                //                }
                //            }
                //        }


                //    }
                //}



                //changes
                string imageUrl = "~/assets/caped.png";

                if (_boxReportModel.boxReportModel?.Count > 0)
                {
                    int counter = 0; // Initialize counter outside the loop
                    int depcount = 0;

                    foreach (var department in _boxReportModel.boxReportModel)
                    {
                        depcount = 0;

                        if (department.boxReportModel?.Count > 0)
                        {
                            foreach (var machine in department.boxReportModel)
                            {
                                if (machine.IsShiftExist)
                                {
                                    if (depcount == 0)
                                        html += $@"<div><b>{department.DepartmentName} </b></div>";

                                    depcount++;
                                    html += $@"<table style='page-break-inside: avoid;'><tr>
                              <td colspan='{machine.boxReportModels?.Count}'>{machine.MachineNumber}</td></tr><tr>";
                                    long shiftIdlast = 0;
                                    foreach (var item in machine.boxReportModels)
                                    {
                                        shiftIdlast = item.Id;
                                        if (item.Id == 0)
                                        {
                                            html += $@"<td style='width: 120px;height: 140px;vertical-align:top;
                                            border: 1px solid black;
                                            margin-top: 0px;
                                            font-size: 11px;
                                            padding: 3px;
                                            line-height: 12px;
                                            font-family: 'Helvetica';'>
                                                <table style='width:100%'>
                                                    <tr>
                                                        <td style='border-bottom:.5px solid black'>{item.RowNum}</td>
                                                        <td style='border-bottom:.5px solid black'></td>
                                                        <td style='border-bottom:.5px solid black'></td>
                                                    </tr>
                                                </table>
                                            </td>";
                                        }
                                        else
                                        {
                                            if(item.shift == "Cancelled")
                                            {
                                                html += $@"<td style='width: 120px;height: 140px;vertical-align:top;
                                                border: 1px solid black;
                                                margin-top: 0px;
                                                font-size: 11px;
                                                padding: 3px;
                                                line-height: 12px;
                                                font-family: 'Helvetica';'>
                                                    <table style='width:100%'>
                                                        <tr>
                                                            <td style='border-bottom:.5px solid black'>{item.RowNum}</td>
                                                            <td style='border-bottom:.5px solid black'></td>
                                                            <td style='border-bottom:.5px solid black'></td>
                                                        </tr>
                                                    </table>
                                                </td>";
                                            }
                                            else
                                            {
                                                string worksiteDetails = item.WorksiteDetails.Length > 20 ? item.WorksiteDetails.Substring(0, 20) + "..." : item.WorksiteDetails;
                                                string tdStyle = (item.shift == "Caped") ? "color:#ff0000;" : "";

                                                html += $@"<td style='
                                                    width: 120px;height: 140px;vertical-align:top;
                                                    border: 1px solid black;
                                                    margin-top: 0px;
                                                    font-size: 11px;
                                                    padding: 3px;
                                                    background:{(item.WorksiteDetails == "Workshops" || item.shift == "Caped" ? "white" : "rgb(192, 255, 192)")}; 
                                                    line-height: 12px;
                                                    font-family: 'Helvetica';
                                                    '>
                                                    {(item.shift == "Caped" ? "<div style='position:absolute; width:120px; text-align:center; top:65px; font-size:24px; opacity:0.5; z-index:1; color:#666666;font-weight: bold;'>CAPED</div>" : "<div></div>")}
                                                    <table style='width:100%'>
                                                        <tr font-family: 'Helvetica'>
                                                            <td>#{item.RowNum}</td>
                                                            <td>{item.PpreDay}</td>
                                                            <td>{item.StartDateTime}</td>
                                                        </tr>
                                                        <tr font-family: 'Helvetica'>
                                                            <td colspan='3' style='{tdStyle}'>{worksiteDetails}</td>
                                                        </tr>
                                                        <tr font-family: 'Helvetica'>
                                                            <td colspan='3'>{item.WorkDescription}</td>
                                                        </tr>
                                                        <tr font-family: 'Helvetica'>
                                                            <td colspan='3'>{(DateTime.Parse(item.StartTime).ToString("HH:mm") + "-" + DateTime.Parse(item.FinishTime).ToString("HH:mm") + "(" + item.CalculatedTime + ")")}</td>
                                                        </tr>
                                                        <tr font-family: 'Helvetica'>
                                                            <td colspan='3'><span>{item.OwnerName}</span> <span>{item.WorksiteELR}</span></td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan='3'>{item.PpreDriver}</td>
                                                        </tr>
                                                        <tr font-family: 'Helvetica'>
                                                            <td colspan='3'>{item.Conductor}</td>
                                                        </tr>
                                                        <tr font-family: 'Helvetica'>
                                                            <td colspan='3' style='padding: 5px;'>{(item.OutShortCode + "    " + item.InShortCode + "    " + item.Id.ToString())}</td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan='3' style='padding: 5px;  font-size: 14px;text-align:right;'>{item.PTONumber}</td>
                                                        </tr>
                                                    </table>
                                                </td>";
                                            }
                                        }
                                    }
                                    html += "</tr></table>";
                                    counter++;
                                }
                            }
                        }
                    }



                    // Calculate the number of empty cells needed in the additional table
                    int emptyCellsCount = 5 - (counter % 5);

                    // Check if additional table is needed and if header should be included
                    if (emptyCellsCount > 0 && counter > 0)
                    {
                        // Add the appropriate number of empty cells
                        string emptyCells = "";
                        for (int i = 0; i < emptyCellsCount; i++)
                        {
                            emptyCells += "<td></td>";
                        }

                        html += $@"<table cellspacing='0' cellpadding='0' style='border: 0pt solid rgb(0, 0, 0); border-collapse: collapse;width:100%;'>
                        <tr font-family: 'Helvetica'>
                            <td>
                                <p style='font-size:14px;text-align:center'>
                                    <span style='font-family:Helvetica'><b>WEEK NO:  {param.WeekNumber} </b></span>
                                </p>
                            </td>
                            <!-- Add other table cells here -->
                            {emptyCells} <!-- Insert additional empty cells here -->
                        </tr>
                    </table>";
                    }
                }

                HtmlConverter.ConvertToDocument(html, pdfDocument, properties);
                pdfDocument.Close();

                pdfWriter.Close();
                return Json(Ok(filePathDate + param.ReportType + "Report.pdf"));
                //var obj = await _contacts.delete(model.Id);
            }
            catch (Exception ex)
            {
                return Json(BadRequest(ex.Message));
            }

        }


        [HttpGet]
        [Route("Reports/ExportWeeklyRoster")]
        public async Task<JsonResult> ExportWeeklyRoster([FromQuery] string Type, int FromWeek, int ToWeek)
        {
            try
            {
                string _RootPath = System.IO.Path.Combine(this.environment.WebRootPath, "ReportsPDF");
                string FilePath = System.IO.Path.Combine(_RootPath, "ExcellCrewRoster.xlsx");
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                RootReportModel resultReport = new RootReportModel();

                resultReport = await _reportServices.GetWeeklyRosterReport(FromWeek, ToWeek, userId);

                if (resultReport?.weeklyRosterModel != null)
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("WeeklyRosterReport");

                        // Set column headers
                        var headers = new string[]
                        {
                "M/C", "Day", "Start Time", "Finish Time", "Location",
                "Sts", "REM", "Path", "From", "To",
                "Crew Manager", "Operators", "conductor", "DRA"
                        };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = headers[i];
                        }

                        // Populate data rows
                        for (int i = 0; i < resultReport?.weeklyRosterModel.Count; i++)
                        {
                            var item = resultReport?.weeklyRosterModel[i];
                            worksheet.Cells[i + 2, 1].Value = item.MachineNum;
                            worksheet.Cells[i + 2, 2].Value = item.PpreDay;
                            worksheet.Cells[i + 2, 3].Value = item.StartTime;
                            worksheet.Cells[i + 2, 4].Value = item.FinishTime;
                            worksheet.Cells[i + 2, 5].Value = item.WorksiteDetails;
                            worksheet.Cells[i + 2, 6].Value = item.STS;
                            worksheet.Cells[i + 2, 7].Value = item.STSREM;
                            worksheet.Cells[i + 2, 8].Value = item.PathTime;
                            worksheet.Cells[i + 2, 9].Value = item.YardOut;
                            worksheet.Cells[i + 2, 10].Value = item.YardIn;
                            worksheet.Cells[i + 2, 11].Value = item.PpreOperator;
                            worksheet.Cells[i + 2, 12].Value = item.PpreDriver;
                            worksheet.Cells[i + 2, 13].Value = item.Contractor;
                            worksheet.Cells[i + 2, 14].Value = item.PpreDra;
                            // Continue populating other properties in a similar manner
                        }

                        // Save the Excel package
                        System.IO.File.WriteAllBytes(FilePath, package.GetAsByteArray());
                    }

                }





                return Json(Ok("ExcellCrewRoster.xlsx"));
                //var obj = await _contacts.delete(model.Id);
            }
            catch (Exception ex)
            {
                return Json(BadRequest(ex.Message));
            }


        }

        static string GenerateUniqueString()
        {
            // Get the current datetime
            DateTime now = DateTime.Now;

            // Format datetime into a string
            string dateTimeString = now.ToString("yyyyMMddHHmmss");

            // Generate a random number (optional)
            Random rnd = new Random();
            int randomNumber = rnd.Next(1000, 9999); // Adjust range as needed

            // Combine datetime string and random number
            string uniqueString = dateTimeString + randomNumber;

            return uniqueString;
        }

        [HttpGet]
        [Route("Reports/GetDistributionList")]
        public async Task<IActionResult> GetDistributionList()
        {
            try
            {
                Dictionary<string, object> response = new Dictionary<string, object>();
                var DistributionList = await distributionServices.GetData("home");
                response.Add("DistributionList", DistributionList);
                return Ok(response);
                //var obj = await _contacts.delete(model.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        public async Task<IActionResult> ChangeLog()
        {
            var identity = User.Identity as ClaimsIdentity;

            // Get the user's object identifier (OID) from the ClaimsPrincipal
            var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            RootReportModel resultReport = new RootReportModel();
            resultReport.WeekList = await _lookup.GetWeeks();
            return View(resultReport);
        }

    }

    public class PDFModelGenerate
    {
        public string ReportHTML { get; set; }
        public string ReportType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string WeekNumber { get; set; }
        public string WeekDateTime { get; set; }
        public int FromWeek { get; set; }
        public string Department { get; set; }
    }

}
