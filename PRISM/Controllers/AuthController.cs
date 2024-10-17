using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.Security.Claims;
using PRISM.Models;
using PRISM.DTO;

namespace PRISM.Controllers
{
	public class AuthController : Controller
	{
		readonly PRISMContext _context;
		public AuthController(PRISMContext prismContext) {
			_context= prismContext;
		}	
		
		public IActionResult Reset()
		{
			return View();
		}
		public IActionResult Login(string ReturnUrl = "/")
		{
			LoginModel objLoginModel = new LoginModel();
			objLoginModel.ReturnUrl = ReturnUrl;
			return View(objLoginModel);
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginModel objLoginModel)
		{
			if (ModelState.IsValid)
			{
				var user = _context.Employees.Where(x => x.UserName == objLoginModel.UserName && x.Password == objLoginModel.Password).FirstOrDefault();
				if (user == null)
				{
					ViewBag.Message = "Invalid Credential";
					return View(objLoginModel);
				}
				else
				{
					var claims = new List<Claim>() {
					new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
						//new Claim(ClaimTypes.Name, user.UserName??""),
						new Claim("FavoriteDrink", "Tea")
					};
					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var principal = new ClaimsPrincipal(identity);
					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
					{
						IsPersistent = objLoginModel.RememberLogin
					});
					return LocalRedirect(objLoginModel.ReturnUrl);
				}
			}
			return View(objLoginModel);
		}
		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return LocalRedirect("/");
		}
	}
}
