using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;

namespace DataTypeMapping.Services
{
    public class AddressService : IAddressService
    {
        private readonly MapApiIdentityContext _identityContext;

        public AddressService(MapApiIdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        public BaseResponse<Address> CreateAddress(AddressDto addressDto, string userId)
        {
            if (addressDto  == null) 
            {
                var error = new List<string> { "Address data is null" };
                return BaseResponse<Address>.Failure(error);
            }
            var address = new Address
            {
                CustomerId = userId,
                Line1 = addressDto.StreetLine1,
                Line2 = addressDto.StreetLine2,
                City = addressDto.City,
                PostalCode = addressDto.PinCode,
                State = addressDto.State,
            };
            try
            {
                _identityContext.Add(address);
                _identityContext.SaveChanges();
                return BaseResponse<Address>.Success(address);
            }
            catch (Exception ex)
            {
                return BaseResponse<Address>.Failure(new List<string> { ex.Message });
            }
        }

        public BaseResponse<AddressDto> DeleteAddress(int id)
        {
            var address = GetAddressById(id);
            var isAdressPresent = address.Data != null && address.IsSuccess == true ;   // to check if address exists
            if(!isAdressPresent)
            {
                return BaseResponse<AddressDto>.Failure(new List<string> { "Address not found" });
            }
            _identityContext.Addresses.Remove(address.Data);
            _identityContext.SaveChanges();
            return BaseResponse<AddressDto>.Success(null);
        }

        public BaseResponse<Address> GetAddressById(int id)
        {
            if (id <= 0)
            {
                var ListError = new List<string> { "Invalid address id" };
                return BaseResponse<Address>.Failure(ListError);
            }
            var address = _identityContext.Addresses.Where(a => a.AddressID == id).FirstOrDefault();
            return address != null ? BaseResponse<Address>.Success(address) : BaseResponse<Address>.Failure(new List<string> { "Address not found" });
        }

        public BaseResponse<List<Address>> GetAllAddresses(string userID)
        {
            var Addresses = _identityContext.Addresses.Where(x => x.CustomerId == userID).ToList();
            if (Addresses == null || Addresses.Count == 0)
            {
                return BaseResponse<List<Address>>.Failure(new List<string> { "Address count zero" });
            }
            return BaseResponse<List<Address>>.Success(Addresses);
        }

        public BaseResponse<Address> UpdateAddress(int id, AddressDto addressDto)
        {
            var address = GetAddressById(id);
            if (!address.IsSuccess || address.Data == null || addressDto == null)
            {
                return BaseResponse<Address>.Failure(new List<string> { "Address or Address data not found" });
            }
            try
            {
                var AddressEntity = address.Data;
                AddressEntity.Line1 = addressDto.StreetLine1;
                AddressEntity.Line2 = addressDto.StreetLine2;
                AddressEntity.City = addressDto.City;
                AddressEntity.State = addressDto.State;
                AddressEntity.PostalCode = addressDto.PinCode;

                _identityContext.Addresses.Update(address.Data);
                _identityContext.SaveChanges();
                return BaseResponse<Address>.Success(address.Data);
            }
            catch (Exception ex)
            {
                return BaseResponse<Address>.Failure(new List<string> { ex.Message });
            }
        }
    }
}
