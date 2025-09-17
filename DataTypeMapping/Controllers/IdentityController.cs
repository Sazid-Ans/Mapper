using AuthServer.Dto.ResponseDto;
using DataTypeMapping.Dto;
using DataTypeMapping.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DataTypeMapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(IUserService userService, IMailService mailService, ILogger<IdentityController> logger)
        {
            _userService = userService;
            _mailService = mailService;
            _logger = logger;
        }
        
        [HttpPost("Register")]
        [ProducesResponseType(typeof(BaseResponse<RegistrationResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegistrationResponse>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<RegistrationResponse>),StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<RegistrationResponse>>> RegisterUser([FromBody] CustomerDto customerDto) 
        {
            
            try
            {
                _logger.LogInformation($"Registration Request Recieved. username: {customerDto.Email}");
                var result = await _userService.RegisterAsync(customerDto);

                if (!result.IsSuccess || result.Data is null)
                {
                    _logger.LogError($"Registration failed. Email: {customerDto.Email} , Reason: {result.Errors.FirstOrDefault().Message}");
                    return BadRequest(result);
                }
                
                string message = $"User registration Success, username:{customerDto.Email}";
                _logger.LogInformation(message);
                await _mailService.SendEmailAsync(result.Data.UserId, "Registration Status", message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration failed. Email: {customerDto.Email} , Reason: {ex.Message}");
                return BaseResponse<RegistrationResponse>.Failure("UnExpected Error", "Something went wrong while Registering");
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> Login(string userName, string passWord)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(passWord))
            {
                return BadRequest(BaseResponse<string>.Failure(
                    StatusCodes.Status400BadRequest.ToString(), "user name or password is empty"));
            }
            try 
            {
               var loginResponse = await _userService.LoginAndGetTokenAsync(userName, passWord);
                if (!loginResponse.IsSuccess &&
                      loginResponse.Errors.Where(err => err.Code.Contains("UserNotFound")).Any())
                {
                    return NotFound(loginResponse);
                }
                if (!loginResponse.IsSuccess && loginResponse.Errors.Any(e=>e.Code.Contains("Incorrect password")))
                {
                    return Unauthorized(loginResponse);
                }
                return Ok(loginResponse);
            }
            catch(Exception ex)
            {
                return BaseResponse<LoginResponse>.Failure(
                    ex.GetType().Name, ex.Message);
            }
        }

        //[HttpPut("UpdatePassword")]
        //public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword) 
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if(string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        //    {
        //        return BadRequest(
        //            BaseResponse<object>.
        //            Failure(new List<string> { "All fields are required" })
        //            );
        //    }
        //    var response = await _userService.UpdatePassword(userId, currentPassword, newPassword);
        //    if (!response.IsSuccess)
        //    {
        //        return BadRequest(response);
        //    }
        //    return Ok(response);
        //}

        //[HttpGet("ForgotPassword/{email}")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ForgotPassword(string email) 
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(email))
        //            return BadRequest(BaseResponse<string>.Failure(new List<string> { "email empty" }));
        //        var baseResponse = await _userService.ForgotPassword(email);
        //        if (!baseResponse.IsSuccess && baseResponse.Errors.Any(err => err.Equals("User not found")))
        //        {
        //            return NotFound(baseResponse);
        //        }
        //        string message = $"Your forgot password token is \n {baseResponse.Data}, \n" +
        //            $"please use the Token for setting new password.";
        //        await _mailService.SendEmailAsync(email, "Forgot Password Token", message);
        //        baseResponse.Data = "Password Reset instructyions sent on registered email, please check.";
        //        return Ok(baseResponse);
        //    }
        //    catch (Exception ex) 
        //    {
        //     return StatusCode(StatusCodes.Status500InternalServerError,
        //        BaseResponse<string>.Failure(
        //            new List<string> { ex.Message }
        //        ));
        //    }
        //}

        //[HttpPost("ResetPassword")]
        //[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<BaseResponse<object>>> ResetPassword(string email, string token, string newPassword)
        //{
        //    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
        //    {
        //        return BadRequest(
        //            BaseResponse<object>.
        //            Failure(new List<string> { "All fields are required" })
        //            );
        //    }
        //    var response = await _userService.ResetPassword(email, token, newPassword);
        //    if (!response.IsSuccess && response.Errors.Any(err => err.Equals("User not found")))
        //    {
        //        return NotFound(response);
        //    }
        //    if (!response.IsSuccess)
        //    {
        //        return BadRequest(response);
        //    }
        //    return Ok(response);
        //}
    }
}
