using Microsoft.AspNetCore.Mvc;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services;
using PRISM.Services.Interfaces;
using System.Reflection.PortableExecutable;

namespace PRISM.Controllers
{
    public class RoutesController : Controller
    {
        private readonly IRoutes _routes;
        private readonly PRISMContext _dbContext;
        public RoutesController(IRoutes routes, PRISMContext pRISMContext)
        {
            this._routes = routes;
            this._dbContext = pRISMContext;
        }
        [Route("Routes")]
        [Route("Routes/index")]
        public async Task<IActionResult> Index()
        {
            var contacts = await _routes.GetData();

            return View(contacts);
        }
        [HttpPost]
        [Route("Routes/insert")]
        public async Task<IActionResult> insert([FromBody] PRISM.Models.Route model)
        {
            try
            {
                var obj = await _routes.Insert(model);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Routes/delete")]
        public async Task<IActionResult> delete([FromBody] CommonId model)
        {
            try
            {
                var obj = await _routes.delete(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
    }
}
