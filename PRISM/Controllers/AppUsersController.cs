using Microsoft.AspNetCore.Mvc;
using PRISM.DTO.AppUserDTO;
using PRISM.DTO.Employeedto;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System.Security.Claims;

namespace PRISM.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly IEmployees _employee;
        private readonly ILookupServices _lookupServices;
        public AppUsersController(IEmployees employee, ILookupServices lookupServices)
        {
            this._employee = employee;
            _lookupServices = lookupServices;
        }
        public async Task<IActionResult> Index()
        {
            AppUserListModel model = new AppUserListModel();
            var appusers = await _employee.GetAppUsers(1);
            model.appUsers = appusers;
            model.userRoles= await _employee.GetRoles();

            return View(model);
        }
		[Route("AppUsers/Roles")]
		public async Task<IActionResult> Roles()
		{
			RoleModels model = new RoleModels();
			model.RoleList = await _employee.GetRoles();

			return View(model);
		}
        [Route("AppUsers/UserLog")]
        public async Task<IActionResult> UserLog()
        {
            var identity = User.Identity as ClaimsIdentity;

            var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var UserLog = await _employee.GetUserLog(userId,0);

            return View(UserLog);
        }
        [HttpGet]
		[Route("AppUsers/GetModuleRoles")]
		public async Task<IActionResult> GetModuleRoles(int RoleId)
		{
			try
			{
				var RoleList = await _employee.GetRolesAndRights(RoleId);
				return Ok(RoleList);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}
		[HttpPost]
		[Route("AppUsers/InsertModuleRoles")]
		public async Task<JsonResult> InsertModuleRoles([FromBody] List<RoleModule> model)
		{
			try
			{
				var obj = await _employee.InsertModuleRole(model);
				return Json(obj);
			}
			catch (Exception ex)
			{
				return Json(ex.Message);
			}

		}
        [HttpPost]
        [Route("AppUsers/insertRole")]
        public async Task<JsonResult> insertRole([FromBody] Role model)
        {
            try
            {
                var obj = await _employee.InsertRole(model);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        [HttpPost]
        [Route("AppUsers/UpdateRole")]
        public async Task<JsonResult> UpdateRole([FromBody] AppUser model)
        {
            try
			{
				var identity = User.Identity as ClaimsIdentity;

				var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var obj = await _employee.UpdateRole(model, userId);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
    }
}
