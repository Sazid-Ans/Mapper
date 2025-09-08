using AuthServer.Dto.ResponseDto;
using DataTypeMapping.Dto;
using DataTypeMapping.Model;

namespace DataTypeMapping.Services.Interface
{
    public interface IAddressService
    {
        public BaseResponse<Address> GetAddressById(int id);
        public BaseResponse<List<Address>> GetAllAddresses(string userId);
        public BaseResponse<AddressResponse> CreateAddress(AddressDto addressDto,string userId);
        public BaseResponse<AddressResponse> UpdateAddress(int id, AddressDto addressDto);
        public BaseResponse<AddressResponse> DeleteAddress(int id);
    }
}
