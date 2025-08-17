using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Identity;

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

        public async Task<CustomerDto> GetCustomer(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var customerDto = Mapper.MapToCustomerDto(user);
            return customerDto;
        }

        public async Task<bool> IsPasswordCorrecrt(string userEmail , string password)
        {
            var user =await _userManager.FindByEmailAsync(userEmail);
            return  await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> IsUserRegistered(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            return user != null;
        }

        public async Task Register(CustomerDto customerDto)
        {
            var customer = Mapper.MapToCustomer(customerDto);
            try
            {
               await _userManager.CreateAsync(customer, customerDto.Password);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }
    }
}
