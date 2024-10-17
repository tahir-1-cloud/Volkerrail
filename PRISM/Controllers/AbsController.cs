using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Nancy.Json;
using PRISM.DTO;
using PRISM.DTO.AbsencesFolder;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System.Security.Claims;

namespace PRISM.Controllers
{
    public class AbsController : Controller
    {
        private readonly IAbsances _absances;
        private readonly IEmployees _employee;
        private readonly ILookupServices _lookupServices;
        public AbsController(IAbsances absances, IEmployees employees, ILookupServices lookupServices)
        {
            _absances = absances;
            _employee = employees;
            _lookupServices = lookupServices;
        }
        public async Task<IActionResult> Index()
        {
            //List<string> strings = new List<string>();
            //strings.Add("Leave");
            EmployeeModel model = new EmployeeModel();
            model.EmployeeList = await _employee.GetData(0);
            model.LeaveTypeList = await _lookupServices.GetLeaves();
           // model.AbsanceList = await _absances.GetData();
            model.type = 0;
            if(model.EmployeeList!=null)
                model.DepartmentList = model.EmployeeList.Select(x => x.Department).ToList();

            return View(model);
        }
        [HttpGet]
        [Route("Abs/GetData")]
        public async Task<IActionResult> GetData([FromQuery] CommonInputModel param)
        {
            try
            {
                var AbsanceList = await _absances.GetData(param.PageIndex, param.PageSize, param.SortColumn, param.SortOrder, param.SearchText,param.EmployeeId);
                return Ok(AbsanceList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPost]
        [Route("Abs/insert")]
        public async Task<IActionResult> insert([FromBody] AbsenceDataModel model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
               
                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var obj = await _absances.Insert(model,userId);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost]
        [Route("Abs/delete")]
        public async Task<IActionResult> delete([FromBody] CommonId model)
        {
            try
            {
                var obj = await _absances.delete(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
    }
}
