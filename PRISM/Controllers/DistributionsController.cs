using Microsoft.AspNetCore.Mvc;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services.Interfaces;

namespace PRISM.Controllers
{
    public class DistributionsController : Controller
    {
        private readonly IDistributions _distribution;
        private readonly ILookupServices _lookupServices;
        public DistributionsController(IDistributions distribution, ILookupServices lookupServices)
        {
            this._distribution = distribution;
            _lookupServices = lookupServices;
        }
        [Route("Distributions")]
        [Route("Distributions/index")]
        public async Task<IActionResult> Index()
        {
            List<string> strings = new List<string>();
            strings.Add("Distribution");
            DistributionModel model = new DistributionModel();
            model.DistributionList = await _distribution.GetData();
            model.LookupList = await _lookupServices.GetLookups(strings);

            return View(model);
        }
        [HttpPost]
        [Route("Distributions/insert")]
        public async Task<IActionResult> insert([FromBody] Distribution model)
        {
            try
            {
                var obj = await _distribution.Insert(model);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Distributions/delete")]
        public async Task<IActionResult> delete([FromBody] CommonId model)
        {
            try
            {
                var obj = await _distribution.delete(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
    }
}
