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
                Addresses = new List<Address> 
                {
                 new Address
                 {
                    Line2 = customerDto.Address.StreetLine2,
                    Line1 = customerDto.Address.StreetLine1,
                    City = customerDto.Address.City,
                    State = customerDto.Address.State,
                    PostalCode = customerDto.Address.PinCode
                 }
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
                    StreetLine2 = customer.Addresses.Select(x=>x.Line2).FirstOrDefault(),
                    StreetLine1 = customer.Addresses.Select(x=>x.Line1).FirstOrDefault(),
                    City = customer.Addresses.Select(x => x.City).FirstOrDefault(),
                    State = customer.Addresses.Select(x => x.State).FirstOrDefault(),
                    PinCode = customer.Addresses.Select(x => x.PostalCode).FirstOrDefault(),
                },
            };
        }
        public static TokenDto MapToTokenDto(Customer customer, IList<string> roles = null, IDictionary<string, string> customClaims = null)
        {
            return new TokenDto
            {
                UserId = customer.Id,
                UserName = customer.UserName,
                Roles = roles,
                CustomClaims = customClaims
            };
        }

        internal static AddressDto MapToAddressDto(Address? address)
        {
            return new AddressDto
            {
                StreetLine1 = address?.Line1,
                StreetLine2 = address?.Line2,
                City = address?.City,
                State = address?.State,
                PinCode = address?.PostalCode
            };
        }

        internal static Address mapToAdressEntity(AddressDto? data, int Id)
        {
            return new Address
            {
                AddressID = Id,
                Line1 = data?.StreetLine1,
                Line2 = data?.StreetLine2,
                City = data?.City,
                State = data?.State,
                PostalCode = data?.PinCode
            };
        }
    }
}
