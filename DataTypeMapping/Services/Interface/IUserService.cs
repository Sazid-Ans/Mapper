using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Customs;
using Microsoft.AspNetCore.Identity;

namespace DataTypeMapping.Services.Interface
{
    public interface IUserService
    {
        Task<IdentityOperationResult> RegisterAsync(CustomerDto customerDto);
        Task<IdentityOperationResult> CreateRoleAsync(string roleName);
        Task<IdentityOperationResult> LoginAsync(string userName, string password);
    }
}
