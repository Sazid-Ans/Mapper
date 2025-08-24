using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Identity;

namespace DataTypeMapping.Services.Interface
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(CustomerDto customerDto);
        Task<(IdentityResult,Customer,string token)> LoginAndGetTokenAsync(string userName, string password);
        //Task<IdentityResult> LogoutAsync(string token);
    }
}
