using Microsoft.AspNetCore.Mvc;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services.Interfaces;

namespace PRISM.Controllers
{
    public class MachinesController : Controller
    {
        private readonly IMachines _machines;
        private readonly ILookupServices _lookupServices;
        public MachinesController(IMachines machines, ILookupServices lookupServices)
        {
            this._machines = machines;
            _lookupServices = lookupServices;
        }
        [Route("Machines")]
        [Route("Machines/index")]
        public async Task<IActionResult> Index()
        {
            List<string> strings = new List<string>();
            strings.Add("MachineDepartment");
			strings.Add("MachineOwner");
			strings.Add("Statuses");
            MachineDetailModel model = new MachineDetailModel();
            model.MachineList = await _machines.GetMachines();
            model.LookupList = await _lookupServices.GetLookups(strings);

            return View(model);
        }
        [HttpPost]
        [Route("Machines/insert")]
        public async Task<IActionResult> insert([FromBody] Machine model)
        {
            try
            {
                var obj = await _machines.Insert(model);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Machines/delete")]
        public async Task<IActionResult> delete([FromBody] CommonId model)
        {
            try
            {
                var obj = await _machines.delete(model.Id);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }
    }
}
