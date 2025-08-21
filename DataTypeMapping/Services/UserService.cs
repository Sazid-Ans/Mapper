using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Customs;
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
       

        public UserService(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                return IdentityOperationResult.AlreadyExists();
            }

            try
            {
                // Step 1: Create user
                var userCreateResult = await _userManager.CreateAsync(customer, customerDto.Password);
                if (!userCreateResult.Succeeded)
                {
                    return IdentityOperationResult.Failed(userCreateResult);
                }

                if (!RoleDto.Roles.Contains(customerDto.Role))
                {
                    return IdentityOperationResult.Failed(
                        IdentityResult.Failed(new IdentityError
                        {
                            Code = "InvalidRole",
                            Description = "The specified role is not valid."
                        })
                    );
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
                return IdentityOperationResult.Created();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // TODO: replace with Serilog in future
                return IdentityOperationResult.Failed(
                    IdentityResult.Failed(new IdentityError
                    {
                        Code = "RegisterException",
                        Description = "An unexpected error occurred during registration."
                    })
                );
            }
        }

        public async Task<IdentityOperationResult> CreateRoleAsync(string roleName) 
        {
            IdentityResult identityResult;
            if (string.IsNullOrWhiteSpace(roleName)) 
            {
                identityResult = IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "EmptyRoleName",
                        Description = "Role name is required."
                    });
                return  IdentityOperationResult.Failed(identityResult);
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
                        identityResult = IdentityResult.Failed(result.Errors.ToArray());
                        return IdentityOperationResult.Failed(identityResult);
                    }
                    return IdentityOperationResult.Created();
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
            return IdentityOperationResult.AlreadyExists();
        }
    }
}
