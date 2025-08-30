using DataTypeMapping.Dto;
using DataTypeMapping.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DataTypeMapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("CreateAddress")]
        public IActionResult CreateAddress([FromBody] AddressDto addressDto) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // pulled from JWT
            var response = _addressService.CreateAddress(addressDto,userId);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpDelete("DeleteAddress/{id}")]
        public IActionResult DeleteAddress(int id) 
        {
            var response = _addressService.DeleteAddress(id);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
        [HttpGet("GetAddressById/{id}")]
        [AllowAnonymous]
        public IActionResult GetAddressById(int id) 
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Unauthorized();
            //}
            var response = _addressService.GetAddressById(id);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
