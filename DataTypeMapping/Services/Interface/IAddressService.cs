using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Utilities;

namespace DataTypeMapping.Services.Interface
{
    public interface IAddressService
    {
        public BaseResponse<Address> GetAddressById(int id);
        public BaseResponse<List<Address>> GetAllAddresses(string userId);
        public BaseResponse<Address> CreateAddress(AddressDto addressDto,string userId);
        public BaseResponse<Address> UpdateAddress(int id, AddressDto addressDto);
        public BaseResponse<AddressDto> DeleteAddress(int id);
    }
}
