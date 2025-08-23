using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Customs;
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

        public UserService(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<bool> IsPasswordCorrectAsync(string userEmail , string password)
        {
            var user =await _userManager.FindByEmailAsync(userEmail);
            return  await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> IsUserRegisteredAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            return user != null;
        }

        public async Task<IdentityOperationResult> RegisterAsync(CustomerDto customerDto)
        {
            var customer = Mapper.MapToCustomer(customerDto);

            // Check if user already exists
            if (await IsUserRegisteredAsync(customerDto.Email))
            {
                return IdentityOperationResult.Ambiguous();
            }

            try
            {
                // Step 1: Create user
                var userCreateResult = await _userManager.CreateAsync(customer, customerDto.Password);
                if (!userCreateResult.Succeeded)
                {
                    return IdentityOperationResult.Failed(userCreateResult);
                }

                // Step 2: Ensure role exists, if not, create it
                var roleResult = await CreateRoleAsync(customerDto.Role);
                if (!roleResult.IdentityResult.Succeeded)
                {
                    return IdentityOperationResult.Failed(roleResult.IdentityResult);
                }

                // Step 3: Add user to role
                var roleAddResult = await _userManager.AddToRoleAsync(customer, customerDto.Role);
                if (!roleAddResult.Succeeded)
                {
                    return IdentityOperationResult.Failed(roleAddResult);
                }

                //verything succeeded
                return IdentityOperationResult.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // TODO: replace with Serilog in future
                return IdentityOperationResult.Failed(
                    IdentityResult.Failed(new IdentityError
                    {
                        Code = "RegistrationException",
                        Description = "An unexpected error occurred during registration."
                    })
                );
            }
        }

        public async Task<IdentityOperationResult> LoginAsync(string userName, string password) 
        {
            IdentityResult identityresult;
            bool isUserRegistered = await IsUserRegisteredAsync(userName);
            if (!isUserRegistered) 
            {
             identityresult = IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "UserNotFound",
                        Description = "User not found."
                    });
                return IdentityOperationResult.Failed(identityresult);
            }
           
           var signInResult = await  _signInManager.CheckPasswordSignInAsync(await _userManager.FindByEmailAsync(userName), password, false);
           if(!signInResult.Succeeded) 
           {
               identityresult = IdentityResult.Failed(
                   new IdentityError
                   {
                       Code = "Incorrect password",
                       Description = "Incorrect password, Retry"
                   });
               return IdentityOperationResult.Failed(identityresult);
           }
            return IdentityOperationResult.Success();
        }

        public async Task<IdentityOperationResult> CreateRoleAsync(string roleName) 
        {
            IdentityResult identityResult;
            bool checkRoleName = RoleDto.Roles.Contains(roleName);
            if (!checkRoleName) 
            {
              return IdentityOperationResult.Failed(
                    IdentityResult.Failed(new IdentityError
                    {
                        Code = IdentityErrorCode.InvalidRole.ToString(),
                        Description = "The specified role is not valid."
                    })
              );
            }
            bool roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist) 
            {
                try
                {
                   IdentityRole role = new IdentityRole(roleName);
                    var result =  await _roleManager.CreateAsync(role);
                    if (!result.Succeeded) 
                    {
                        identityResult = IdentityResult.Failed(
                            new IdentityError { Code = IdentityErrorCode.Unknown.ToString(), Description = result.Errors.FirstOrDefault().Description});
                        return IdentityOperationResult.Failed(identityResult);
                    }
                    return IdentityOperationResult.Success();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex); //will add serilog in next sprint
                    identityResult = IdentityResult.Failed(new IdentityError
                    {
                        Code = "RoleCreateException",
                        Description = "An unexpected error occurred while creating the role."
                    });
                    return IdentityOperationResult.Failed(identityResult);
                }
            }
            return IdentityOperationResult.Ambiguous();
        }
    }
}
