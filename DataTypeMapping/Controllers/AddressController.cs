using AuthServer.Dto.ResponseDto;
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
            var response = _addressService.CreateAddress(addressDto, userId);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpDelete("DeleteAddress{id}")]
        public IActionResult DeleteAddress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addressList = _addressService.GetAllAddresses(userId);
            var isAddressPresent = addressList.Data != null && addressList.Data.Any(a => a.AddressID == id);
            if (!isAddressPresent)
            {
                var error = addressList.Errors.FirstOrDefault();
                return BadRequest(BaseResponse<AddressDto>.Failure(error.Code , error.Message));
            }
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
            var response = _addressService.GetAddressById(id);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllAddress")]
        public IActionResult GetAllAddress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = _addressService.GetAllAddresses(userId);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateAddress{id}")]
        public IActionResult UpdateAddress(int id, [FromBody] AddressDto addressDto)
        {
            if (id == 0 || addressDto == null)
            {
                return BadRequest(BaseResponse<AddressDto>.Failure("BadRequest","Please check your request and retry."));
            }
            var Baseresponse = _addressService.UpdateAddress(id, addressDto);
            if (!Baseresponse.IsSuccess)
            {
                return BadRequest(Baseresponse);
            }
            return Ok(Baseresponse);
        }
    }
}
