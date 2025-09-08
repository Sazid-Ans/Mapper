using AuthServer.Dto.ResponseDto;
using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Model.Enum;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataTypeMapping.Services
{
    public class UserService : IUserUtilityService, IUserService
    {
        public readonly UserManager<Customer> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly SignInManager<Customer> _signInManager;
        public readonly IJwtService _jwtService;
        public readonly MapApiDbContext _dbContext;
        public readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager, IJwtService jwtService, MapApiDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<RegistrationResponse>> RegisterAsync(CustomerDto customerDto)
        {
            // 1. Check if user already exists
            var user = await _userManager.FindByEmailAsync(customerDto.Email);
            if (user is not null)
            {
                return BaseResponse<RegistrationResponse>.Failure(
                    IdentityErrorCode.UserAlreadyExists.ToString(),"A user with this email already exists.");
            }

            // 2. Validate roles
            if(customerDto.Roles.Count == 0) 
            {
                customerDto.Roles.Add("Customer");
            }
            var rolesToAssign = customerDto.Roles.Distinct().ToList();
            var checkRolesExists = rolesToAssign.All(r => RoleDto.Roles.Contains(r,StringComparer.OrdinalIgnoreCase));
            if(!checkRolesExists) 
            {
                return BaseResponse<RegistrationResponse>.Failure(
                    IdentityErrorCode.InvalidRole.ToString(), "One or more specified roles are invalid.");
            }
            var customer = Mapper.MapToCustomer(customerDto);

            // 4. Start Transaction (EF Core style)
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                // 5. Create User
                var userCreateResult = await _userManager.CreateAsync(customer, customerDto.Password);
                if (!userCreateResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    var identityError = userCreateResult.Errors.FirstOrDefault();
                    return BaseResponse<RegistrationResponse>.Failure(
                        identityError.Code, identityError.Description) ;
                }

                // 6. Assign Roles
                if (rolesToAssign.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(customer, rolesToAssign);
                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        var identityError = roleResult.Errors.FirstOrDefault();

                    var errorDetail = new List<ErrorDetail>{
                    new ErrorDetail
                    {
                        Code = identityError.Code, Message = identityError.Description
                    }
                    };
                        return BaseResponse<RegistrationResponse>.Failure(
                            identityError.Code, identityError.Description);
                    }
                }

                // 7. Commit if everything succeeds
                await transaction.CommitAsync();
                var registrationResponse = new RegistrationResponse
                {
                    UserName = customer.UserName,
                    UserId = customer.Id,
                    Roles = rolesToAssign,
                };
                return BaseResponse<RegistrationResponse>.Success(registrationResponse);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BaseResponse<RegistrationResponse>.Failure(
                    ex.GetType().Name, ex.Message);
            }
        }

        public async Task<BaseResponse<LoginResponse>> LoginAndGetTokenAsync(string userMail, string password) 
        {
            var user = await _userManager.FindByEmailAsync(userMail);
            if (user is not null)
            {
                return BaseResponse<LoginResponse>.Failure(
                    "UserNotFound", $"User not found, entered user {userMail}");
            }
           
           var signInResult = await  _signInManager.CheckPasswordSignInAsync(user, password, false);
           if(!signInResult.Succeeded) 
           {
                return BaseResponse<LoginResponse>.Failure(
                    "Incorrect password", "Incorrect password, Please Retry.");
           }
            var tokenDto = new TokenDto
            {
                UserId = user.Id,
                UserName = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };
            var token = _jwtService.GenerateToken(tokenDto);

            return BaseResponse<LoginResponse>.Success(
                new LoginResponse { JwtToken = token.Data, Username = tokenDto.UserName }
                );
        }

        public void SetCurrentUser(Customer user)
        {
            _httpContextAccessor.HttpContext.Items["User"] = user;
        }

        public Customer GetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext.Items.TryGetValue("User", out var userObj) && userObj is Customer user)
            {
                return user;
            }
            return null;
        }

        public async Task<BaseResponse<string>> UpdatePassword(string userEmail, string currentPassword, string newPassword) 
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is not null)
            {
                return BaseResponse<string>.Failure(
                    "UserNotFound", $"User Not Found or entered username: {userEmail} incorrect");
            }
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.Select(e => e).FirstOrDefault();
                return BaseResponse<string>.Failure(
                    error.Code , error.Description);
            }
            return BaseResponse<string>.Success("Password updated Successfully");

        }
        
        public async Task<BaseResponse<string>> ForgotPassword(string userEmail) 
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BaseResponse<string>.Failure(
                     "UserNotFound" , $"User not found, entered User: {userEmail}");
            }
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                return BaseResponse<string>.Success(token);
            }
            catch (Exception ex) 
            {
                return BaseResponse<string>.Failure(
                    ex.GetType().Name , ex.Message);
            }
        }
        public async Task<BaseResponse<string>> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return BaseResponse<string>.Failure(
                    "UserNotFound", $"User not found, entered User: {email}");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.Select(e => e).FirstOrDefault();
                return BaseResponse<string>.Failure(
                    error.Code, error.Description);
            }

            return BaseResponse<string>.Success("Password reset successfully");
        }
    }
}
