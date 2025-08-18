
using Microsoft.AspNetCore.Identity;

namespace DataTypeMapping.Services.Interface
{
    public interface IUserCheckService
    {
        Task<bool> IsUserRegisteredAsync(string userEmail);
        Task<bool> IsPasswordCorrectAsync(string userEmail, string password);
    }
}
