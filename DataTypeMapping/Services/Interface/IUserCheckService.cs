
namespace DataTypeMapping.Services.Interface
{
    public interface IUserCheckService
    {
        Task<bool> IsUserRegistered(string userEmail);
        Task<bool> IsPasswordCorrecrt(string userEmail, string password);
    }
}
