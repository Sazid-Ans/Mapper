using DataTypeMapping.Dto;
using DataTypeMapping.Model;

namespace DataTypeMapping.Utilities
{
    public static class Mapper
    {
        public static Customer MapToCustomer(CustomerDto customerDto)
        {
            return new Customer
            {
                Email = customerDto.Email,
                UserName = customerDto.Name,
                PhoneNumber = customerDto.PhoneNumber,
                PasswordHash = customerDto.Password,
                Address = new Address
                {
                    Line1 = customerDto.Address.StreetLine1,
                    Line2 = customerDto.Address.StreetLine2,
                    City = customerDto.Address.City,
                    State = customerDto.Address.State,
                    PostalCode = customerDto.Address.PinCode
                },
            };
        }

        public static CustomerDto MapToCustomerDto(Customer customer)
        {
            return new CustomerDto
            {
                Email = customer.Email,
                Name = customer.UserName,
                PhoneNumber = customer.PhoneNumber,
                Password = customer.PasswordHash,
                Address = new AddressDto
                {
                    StreetLine2 = customer.Address.Line2,
                    StreetLine1 = customer.Address.Line1,
                    City = customer.Address.City,
                    State = customer.Address.State,
                    PinCode = customer.Address.PostalCode
                },
            };
        }
    }
}
