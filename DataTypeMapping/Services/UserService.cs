using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Model.Enum;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Identity;

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

        public async Task<bool> IsPasswordCorrectAsync(string userEmail , string password)
        {
            var user =await _userManager.FindByEmailAsync(userEmail);
            return  await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<(bool, Customer)> IsUserRegisteredAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            return (user != null, user);
        }

        public async Task<IdentityResult> RegisterAsync(CustomerDto customerDto)
        {
            var identityResult = new IdentityResult();

            // 1. Check if user already exists
            var (isRegistered, user) = await IsUserRegisteredAsync(customerDto.Email);
            if (isRegistered && user != null)
            {
                 identityResult = IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = IdentityErrorCode.UserAlreadyExists.ToString(),
                        Description = "A user with this email already exists."
                    });
                return identityResult;
            }

            // 2. Validate roles
            var rolesToAssign = customerDto.Roles.Distinct().ToList();
            var checkRolesExists = rolesToAssign.All(r => RoleDto.Roles.Contains(r,StringComparer.OrdinalIgnoreCase));
            if(!checkRolesExists) 
            {
                identityResult = IdentityResult.Failed(
                   new IdentityError
                   {
                       Code = IdentityErrorCode.InvalidRole.ToString(),
                       Description = "One or more specified roles are invalid."
                   });
                return identityResult;
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
                    identityResult = IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "UserCreationFailed",
                            Description = userCreateResult.Errors.FirstOrDefault()?.Description
                        });
                    return identityResult;
                }

                // 6. Assign Roles
                if (rolesToAssign.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(customer, rolesToAssign);
                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return roleResult;
                    }
                }

                // 7. Commit if everything succeeds
                await transaction.CommitAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                // Rollback on exception
                await transaction.RollbackAsync();

                return IdentityResult.Failed(new IdentityError
                {
                    Code = IdentityErrorCode.InternalServerError.ToString(),
                    Description = $"Unexpected error occurred: {ex.Message}"
                });
            }
        }

        public async Task<(IdentityResult,string token)> LoginAndGetTokenAsync(string userName, string password) 
        {
            IdentityResult identityresult;
            var (isUserRegistered, user) = await IsUserRegisteredAsync(userName);
            if (!isUserRegistered && user == null) 
            {
             identityresult = IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "UserNotFound",
                        Description = "User not found."
                    });
                return (identityresult, string.Empty);
            }
           
           var signInResult = await  _signInManager.CheckPasswordSignInAsync(user, password, false);
           if(!signInResult.Succeeded) 
           {
               identityresult = IdentityResult.Failed(
                   new IdentityError
                   {
                       Code = "Incorrect password",
                       Description = "Incorrect password, Please Retry"
                   });
               return (identityresult, string.Empty);
           }
           var tokenDto = new TokenDto
           {
               UserId = user.Id,
               UserName = user.Email,
               Roles = await _userManager.GetRolesAsync(user)
           };
            var token = _jwtService.GenerateToken(tokenDto);

            SaveUser(user);

            return (IdentityResult.Success, token.Data);
        }

        public void  SaveUser(Customer user)
        {
            _httpContextAccessor.HttpContext.Items["User"] = user;
        }

        public Customer FetchUser()
        {
            if (_httpContextAccessor.HttpContext.Items.TryGetValue("User", out var userObj) && userObj is Customer user)
            {
                return user;
            }
            return null;
        }

        public async Task<BaseResponse<string>> UpdatePassword(string userEmail, string currentPassword, string newPassword) 
        { 
            var (isUserRegistered, user) = await IsUserRegisteredAsync(userEmail);
            if (!isUserRegistered && user == null)
            {
                return BaseResponse<string>.Failure(new List<string> { "User not found" });
            }
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BaseResponse<string>.Failure(errors);
            }
            return BaseResponse<string>.Success("Password updated Successfully");

        }

        public async Task<BaseResponse<string>> ForgotPassword(string userEmail) 
        {
            var (isUserRegistered, user) = await IsUserRegisteredAsync(userEmail);
            if (!isUserRegistered && user == null)
            {
                return BaseResponse<string>.Failure(new List<string> { "User not found" });
            }
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                return BaseResponse<string>.Success(token);
            }
            catch (Exception ex) 
            {
                return BaseResponse<string>.Failure(new List<string> { ex.Message });
            }
        }
        public async Task<BaseResponse<string>> ResetPassword(string email, string token, string newPassword)
        {
            var (isUserRegistered, user) = await IsUserRegisteredAsync(email);
            if (!isUserRegistered || user == null)
            {
                return BaseResponse<string>.Failure(new List<string> { "User not found" });
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BaseResponse<string>.Failure(errors);
            }

            return BaseResponse<string>.Success("Password reset successfully");
        }
    }
}
