using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Utilities;

namespace DataTypeMapping.Services.Interface
{
    public interface IAddressService
    {
        public BaseResponse<Address> GetAddressById(int id);
        public BaseResponse<List<Address>> GetAllAddresses();
        public BaseResponse<Address> CreateAddress(AddressDto addressDto);
        public BaseResponse<Address> UpdateAddress(int id, AddressDto addressDto);
        public BaseResponse<Address> DeleteAddress(int id);
    }
}
