using DataTypeMapping.Dto;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Identity;

namespace DataTypeMapping.Services.Interface
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(CustomerDto customerDto);
        Task<(IdentityResult,string token)> LoginAndGetTokenAsync(string userName, string password);
        Task<BaseResponse<string>> UpdatePassword(string userEmail, string currentPassword, string newPassword);
        Task<BaseResponse<string>> ForgotPassword(string userEmail);
        Task<BaseResponse<string>> ResetPassword(string userEmail, string token, string newPassword);

        //Task<IdentityResult> LogoutAsync(string token);
    }
}
