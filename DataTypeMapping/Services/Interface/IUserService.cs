using DataTypeMapping.Dto;
using DataTypeMapping.Model;

namespace DataTypeMapping.Services.Interface
{
    public interface IUserService
    {
        Task<CustomerDto> GetCustomer(string email);
        Task Register(Customer customer);

    }
}
