using DataTypeMapping.Model;
using DataTypeMapping.Utilities;

namespace DataTypeMapping.Services.Interface
{
    public interface IUserUtilityService
    {
        void SetCurrentUser(Customer user);
        Customer GetCurrentUser();
    }
}
