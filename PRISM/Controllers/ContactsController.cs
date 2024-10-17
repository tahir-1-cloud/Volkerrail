using Microsoft.AspNetCore.Mvc;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services;
using PRISM.Services.Interfaces;
using System.Reflection.PortableExecutable;

namespace PRISM.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContacts _contacts;
        private readonly PRISMContext _dbContext;
		private readonly IHomeServices _homeServices;
		public ContactsController(IContacts contacts, PRISMContext pRISMContext,IHomeServices homeServices)
        {
            this._contacts = contacts;
            this._dbContext = pRISMContext;
            this._homeServices = homeServices;
        }
        [Route("Contacts")]
        [Route("Contacts/index")]
        public async Task<IActionResult> Index(string alphabat)
        {
            var contacts = await _contacts.GetData(alphabat);

            return View(contacts);
        }
        [HttpPost]
        [Route("Contacts/insert")]
        public async Task<IActionResult> insert([FromBody] Contact model)
        {
            try
            {
                var obj = await _contacts.Insert(model);
				var coontactlist = await _contacts.GetData("");
				return Ok(coontactlist);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Contacts/delete")]
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
    }
}
