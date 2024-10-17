using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services;
using PRISM.Services.Interfaces;
using System.IO.Pipes;
using System.Linq;
using System.Security.Claims;

namespace PRISM.Controllers
{
    public class LookupsController : Controller
    {
        private readonly ILookupServices _lookupServices;
        private readonly PRISMContext _dbContext;
        public LookupsController(ILookupServices lookupServices,PRISMContext pRISMContext)
        {
            this._lookupServices = lookupServices;
            this._dbContext = pRISMContext;

		}

		[Route("Lookups")]
		[Route("Lookups/index")]
		public async Task<IActionResult> Index(string type)
		{
			List<string> strings = new List<string>();
			strings.Add(type);
            var list = await _lookupServices.GetLookups(strings);
			return View(list);
		}
        [Route("Lookups/TemplateSetting")]
        public async Task<IActionResult> TemplateSetting()
        {
            return View();
        }
        [HttpPost]
		[Route("Lookups/insert")]
		public async Task<IActionResult> insert([FromBody] LookupEntity model)
		{
			try
			{
				var obj = await _lookupServices.Insert(model);
				return Ok(obj);
			}
			catch (Exception ex)
			{
				return BadRequest();
			}

		}

		[HttpPost]
		[Route("Lookups/delete")]
		public async Task<IActionResult> delete([FromBody] CommonId model)
		{
			try
			{
				var obj = await _lookupServices.delete(model.Id);
				return Ok(obj);
			}
			catch (Exception ex)
			{
				return BadRequest();
			}
			

		}
        [Route("Lookups/Weeks")]
        public async Task<IActionResult> Weeks()
        {
            
            var list = await _lookupServices.GetWeeks();
            return View(list);
        }
        [HttpGet]
        [Route("Lookups/GetTemplateData")]
        public async Task<IActionResult> GetTemplateData()
        {
            try
            {
                var list = await _lookupServices.GetTemplateData();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        [Route("Lookups/LeaveTypes")]
        public async Task<IActionResult> LeaveTypes()
        {

            var list = await _lookupServices.GetLeaves();
            return View(list);
        }
       
        [HttpPost]
        [Route("Lookups/insertWeek")]
        public async Task<IActionResult> insertWeek([FromBody] WeeksModel model)
        {
            try
            {
                var obj = await _lookupServices.Insert(model);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Lookups/deleteWeek")]
        public async Task<IActionResult> deleteWeek([FromBody] CommonId model)
        {
            try
            {
                var obj = await _lookupServices.deleteWeek(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
        [HttpPost]
        [Route("Lookups/insertLeave")]
        public async Task<IActionResult> insertLeave([FromBody] LeaveType model)
        {
            try
            {
                var obj = await _lookupServices.Insert(model);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Lookups/deleteLeave")]
        public async Task<IActionResult> deleteLeaveType([FromBody] CommonId model)
        {
            try
            {
                var obj = await _lookupServices.deleteLeave(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
        [HttpPost]
        [Route("Lookups/InsertTemplate")]
        public async Task<IActionResult> InsertTemplate([FromBody] ShiftTemplate model)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                // Get the user's object identifier (OID) from the ClaimsPrincipal
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Use the userId as needed
                if (userId != null)
                {
                    model.UserId= userId;
                    // Your logic here
                }
               
                var obj = await _lookupServices.InsertTemplate(model);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Lookups/DeleteTemplate")]
        public async Task<IActionResult> DeleteTemplate([FromBody] CommonId model)
        {
            try
            {
                var obj = await _lookupServices.DeleteTemplate(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
    }
}
