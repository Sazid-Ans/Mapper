using DataTypeMapping.Dto;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DataTypeMapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;

        public IdentityController(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
        }
        [HttpPost("Register")]
        [ProducesResponseType(typeof(BaseResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                    throw new Exception(message, ex); //not right, will have to change this.
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
               var (loginResult,token) =await _userService.LoginAndGetTokenAsync(userName, passWord);
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

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                return BadRequest(
                    BaseResponse<object>.
                    Failure(new List<string> { "All fields are required" })
                    );
            }
            var response = await _userService.UpdatePassword(userId, currentPassword, newPassword);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        

        [HttpGet("ForgotPassword/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email) 
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(BaseResponse<string>.Failure(new List<string> { "email empty" }));
                var baseResponse = await _userService.ForgotPassword(email);
                if (!baseResponse.IsSuccess && baseResponse.Errors.Any(err => err.Equals("User not found")))
                {
                    return NotFound(baseResponse);
                }
                string message = $"Your forgot password token is \n {baseResponse.Data}, \n" +
                    $"please use the Token as temp password and Reset it after logging in.";
                await _mailService.SendEmailAsync(email, "Forgot Password Token", message);
                baseResponse.Data = "Password Reset instructyions sent on registered email, please check.";
                return Ok(baseResponse);
            }
            catch (Exception ex) 
            {
             return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<string>.Failure(
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            {
                return BadRequest(
                    BaseResponse<object>.
                    Failure(new List<string> { "All fields are required" })
                    );
            }
            var response = await _userService.ResetPassword(email, token, newPassword);
            if (!response.IsSuccess && response.Errors.Any(err => err.Equals("User not found")))
            {
                return NotFound(response);
            }
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
