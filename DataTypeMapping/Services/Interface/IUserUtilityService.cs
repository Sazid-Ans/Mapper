
using DataTypeMapping.Model;

namespace DataTypeMapping.Services.Interface
{
    public interface IUserUtilityService
    {
        Task<(bool, Customer)> IsUserRegisteredAsync(string userEmail);
        Task<bool> IsPasswordCorrectAsync(string userEmail, string password);
        void SaveUser(Customer user);
        Customer FetchUser();
    }
}
