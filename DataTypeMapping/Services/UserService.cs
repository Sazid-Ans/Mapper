using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Model.Enum;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DataTypeMapping.Services
{
    public class UserService : IUserCheckService, IUserService
    {
        public readonly UserManager<Customer> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly SignInManager<Customer> _signInManager;
        public readonly IJwtService _jwtService;
        public readonly MapApiDbContext _dbContext;

        public UserService(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager, IJwtService jwtService, MapApiDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _dbContext = dbContext;
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

        public async Task<(IdentityResult, Customer,string token)> LoginAndGetTokenAsync(string userName, string password) 
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
                return (identityresult, user, null);
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
               return (identityresult,null, null);
           }
           var tokenDto = new TokenDto
           {
               UserId = user.Id,
               UserName = user.Email,
               Roles = await _userManager.GetRolesAsync(user)
           };
            var token = _jwtService.GenerateToken(tokenDto);
            return (IdentityResult.Success, user, token);
        }

    }
}
