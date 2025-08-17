using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DataTypeMapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] CustomerDto customerDto) 
        {
            if (customerDto == null) 
            {
                return BadRequest();
            }

            return Content(JsonConvert.SerializeObject(customerDto));
        } 
        
    }
}
