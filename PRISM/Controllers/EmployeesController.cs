using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PRISM.DTO;
using PRISM.DTO.Employeedto;
using PRISM.Models;
using PRISM.Services.Interfaces;

namespace PRISM.Controllers
{
    public class EmployeesController : Controller
    {
        

		private readonly IEmployees _employee;
		private readonly ILookupServices _lookupServices;
		public EmployeesController(IEmployees employee, ILookupServices lookupServices)
		{
			this._employee = employee;
			_lookupServices = lookupServices;
		}
		[Route("Employees")]
		[Route("Employees/index")]
		public async Task<IActionResult> Index(string SearchText,string type="")
		{
			List<string> strings = new List<string>();
			strings.Add("Employee");
            strings.Add("MachineDepartment");
            EmployeeModel model = new EmployeeModel();
			model.EmployeeList = await _employee.GetData(0, SearchText);
			model.LookupList = await _lookupServices.GetLookups(strings);
			model.EmpType = type;

			return View(model);
		}
        
		[HttpPost]
		[Route("Employees/insert")]
		public async Task<IActionResult> insert([FromBody] Employee model)
		{
			try
			{
				var obj = await _employee.Insert(model);
				return Ok(obj);
			}
			catch (Exception ex)
			{
				return BadRequest();
			}

		}

		[HttpPost]
		[Route("Employees/delete")]
		public async Task<IActionResult> delete([FromBody] CommonId model)
		{
			try
			{
				var obj = await _employee.delete(model.Id);
				return Ok(obj);
			}
			catch (Exception ex)
			{
				return BadRequest();
			}


		}
      
    }
}
