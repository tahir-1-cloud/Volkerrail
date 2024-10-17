using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRISM.DTO.APIModels;
using PRISM.Filters;
using PRISM.Services;
using PRISM.Services.Interfaces;
using System.Globalization;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRISM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class V1Controller : ControllerBase
    {
        private readonly IApiServices _apiServices;
        public V1Controller(IApiServices apiServices)
        {
            this._apiServices = apiServices;
        }
        // GET: api/<V1Controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpGet]
        [Route("delta-machine-shifts")]
        public async Task<IActionResult> deltamachineshifts(DateTime StartDate, DateTime? endDate)
        {
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];

                // Check if the header is present and is of the correct scheme (Basic)
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the base64-encoded credentials part
                    string encodedCredentials = authHeader.Substring("Bearer ".Length).Trim();

                    // Decode the base64-encoded credentials
                    byte[] credentialsBytes = Convert.FromBase64String(encodedCredentials);
                    string decodedCredentials = Encoding.UTF8.GetString(credentialsBytes);

                    // Split the decoded credentials into username and password
                    string[] credentials = decodedCredentials.Split(':');
                    string username = credentials[0];
                    string password = credentials[1];

                    // Now, you have the username and password for further processing
                    // ...
                    if (username == "VolkerRail" && password == "VolderRail?123")
                    {
                        var list = await this._apiServices.GetShifts(StartDate, endDate);
                        Dictionary<string, object> response = new Dictionary<string, object>();
                        response.Add("deltaMachineShifts", list);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                    // Authorization header is either missing or not in the correct format
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
          
           
        }
       

        // GET api/<V1Controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<V1Controller>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<V1Controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<V1Controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
