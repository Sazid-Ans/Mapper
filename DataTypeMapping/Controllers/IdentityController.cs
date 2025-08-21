using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DataTypeMapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        public readonly IUserService _userService;
        public IdentityController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] CustomerDto customerDto) 
        {
            if (customerDto == null) 
            {
                return BadRequest();
            }
            try
            {
                var result = await _userService.RegisterAsync(customerDto);
                if (!result.IdentityResult.Succeeded)
                {
                    return BadRequest(BaseResponse<CustomerDto>
                        .Failure(result.IdentityResult.Errors.Select(e => e.Description).ToList(), "User registration failed."));
                }
                else if (result.WasCreated)
                {
                    return Ok(BaseResponse<CustomerDto>
                        .Success(customerDto, "User registered successfully."));

                }
                else
                {
                    return Conflict(BaseResponse<CustomerDto>
                        .Failure(new List<string> { "User already exists." }, "User registration failed."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<CustomerDto>.Failure(
                    new List<string> { ex.Message },
                    "An error occurred during registration."
                ));
            }
        } 
    }
}
