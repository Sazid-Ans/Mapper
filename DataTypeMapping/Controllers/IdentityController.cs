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
        public readonly IMailService _mailService;
        public IdentityController(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
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
                string message = "";
                var result = await _userService.RegisterAsync(customerDto);
                IActionResult response;

                if (!result.IdentityResult.Succeeded)
                {
                    message = $"User registration failed.";
                    response = BadRequest(BaseResponse<CustomerDto>
                        .Failure(result.IdentityResult.Errors.Select(e => e.Description).ToList(), message));
                }
                else if (result.WasCreated)
                {
                    message = $"User registration Success username:{customerDto.Email}";
                     response = Ok(BaseResponse<CustomerDto>
                        .Success(customerDto, "User registered successfully."));
                }
                else
                {
                    message = $"user already exists, user registraion failed. Name: {customerDto.Name}, Username: {customerDto.Email}";
                    response = Conflict(BaseResponse<CustomerDto>
                        .Failure(new List<string> { "User already exists." }, "User registration failed."));
                }
                try
                {
                    await _mailService.SendEmailAsync(customerDto.Email, "Registration Status", message);
                }
                catch (Exception ex) 
                {
                    throw new Exception(message, ex);
                }
                return response;
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
