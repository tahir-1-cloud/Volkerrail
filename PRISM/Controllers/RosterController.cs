using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services;
using PRISM.Services.Interfaces;
using System.Diagnostics;

namespace PRISM.Controllers
{
    [Authorize]
    public class RosterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoster _roster;
        private readonly ILookupServices _lookupServices;
        private readonly IEmployees _employee;
        private readonly IMachines _machines;
        private readonly IHomeServices _homeServices;
        public RosterController(ILogger<HomeController> logger, IRoster roster, ILookupServices lookupServices, IEmployees employees, IMachines machines, IHomeServices homeServices)
        {
            _logger = logger;
            _roster = roster;
            _lookupServices = lookupServices;
            _employee = employees;
            _machines = machines;
            _homeServices = homeServices;   
        }

        public async Task<IActionResult> Index()
        {
            RosterViewModel rosterViewModel= new RosterViewModel();
            var weekList = await _lookupServices.GetWeeks();
            var MachineList= await _machines.GetMachines();

            List<string> strings = new List<string>();
            //strings.Add("Employee");
            strings.Add("MachineDepartment");
            var LookupList = await _lookupServices.GetLookups(strings);

            if (LookupList != null)
                rosterViewModel.DepartmentList = LookupList.Select(x => x.Name).ToList();

            rosterViewModel.WeekList = weekList;
            return View(rosterViewModel);
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("Roster/GetData")]
        public async Task<IActionResult> GetData([FromQuery] string department,int FromWeek,int ToWeek)
        {
            try
            {
                var EmployeeList = await _employee.GetData(0);
                var DepartmentList = EmployeeList.Select(x => x.Department).Distinct().ToList();
                if (!string.IsNullOrWhiteSpace(department)) {
                    EmployeeList = EmployeeList.Where(x => x.Department == department).ToList();
                }


                var weeklyComments = await _homeServices.GetWeeklyComments(FromWeek);


                Dictionary<string, object> response = new Dictionary<string, object>();
                //var obj = await _homeServices.GetData();
                List<string> strings = new List<string>();
                var lookups = await _lookupServices.GetLookups(strings);
                var rosterdata = await _roster.GetData(0,FromWeek,ToWeek);
                // response.Add("ShiftData", obj);
                response.Add("LookupData", lookups);
                response.Add("DepartmentData", DepartmentList);
                response.Add("EmployeeData", EmployeeList);
                response.Add("RosterData", rosterdata);
                response.Add("WeeklyComments", weeklyComments);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("Roster/SaveShiftRosterDescription")]
        public async Task<JsonResult> SaveShiftRosterDescription([FromBody] ShiftRosterDetail param)
        {
            try
            {
                var result =await _roster.SaveShiftRosterDescription(param.ShiftId??0,param.RosterShiftDescription??"");
                return Json(Ok(result));
            }
            catch (Exception ex)
            {
                return Json(BadRequest(ex.Message));
            }

        }
    }
}
