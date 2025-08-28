using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataTypeMapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly IJwtService _jwtService;

        public IdentityController(IUserService userService, IMailService mailService, IJwtService jwtService)
        {
            _userService = userService;
            _mailService = mailService;
            _jwtService = jwtService;
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

                if (!result.Succeeded)
                {
                    message = result.Errors.Select(e => e.Code).FirstOrDefault();
                    return BadRequest(BaseResponse<CustomerDto>
                        .Failure(result.Errors.Select(e => e.Description).ToList()));
                }
                
                message = $"User registration Success username:{customerDto.Email}";
                try
                {
                    await _mailService.SendEmailAsync(customerDto.Email, "Registration Status", message);
                }
                catch (Exception ex) 
                {
                    throw new Exception(message, ex);
                }
                return Ok(BaseResponse<object>
                        .Success(new { userName = customerDto.Email}));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<CustomerDto>.Failure(
                    new List<string> { ex.Message }
                ));
            }
        } 

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string userName, string passWord)
        {
            if(string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(passWord))
            {
                return BadRequest(
                    BaseResponse<object>.
                    Failure(new List<string> { "Username and password must be provided." })
                    );
            }
            try 
            {
               var (loginResult,cust,token) =await _userService.LoginAndGetTokenAsync(userName, passWord);
                if (!loginResult.Succeeded &&
                      loginResult.Errors.Any(e => e.Code.Contains("UserNotFound")))
                {
                    return NotFound(
                        BaseResponse<object>.Failure(
                            loginResult.Errors.Select(e => e.Description).ToList()
                        ));
                }
                if (!loginResult.Succeeded && loginResult.Errors.Any(e=>e.Code.Contains("Incorrect password")))
                {
                    return Unauthorized(
                        BaseResponse<object>.
                        Failure(loginResult.Errors.Select(e => e.Description).ToList())
                        );
                }
                return Ok(
                    BaseResponse<object>.
                    Success(new { JwtToken = token })
                    );
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<object>.Failure(
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}
