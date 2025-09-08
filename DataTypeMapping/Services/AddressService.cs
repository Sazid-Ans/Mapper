using AuthServer.Dto.ResponseDto;
using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Services.Interface;

namespace DataTypeMapping.Services
{
    public class AddressService : IAddressService
    {
        private readonly MapApiIdentityContext _identityContext;

        public AddressService(MapApiIdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        public BaseResponse<AddressResponse> CreateAddress(AddressDto addressDto, string userId)
        {
            if (addressDto  == null) 
            {
                return BaseResponse<AddressResponse>.Failure(
                    "BadRequest", "Address payload empty.");
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
                 var addressEntity = _identityContext.Add(address).Entity;
                _identityContext.SaveChanges();

                return BaseResponse<AddressResponse>.Success(new AddressResponse
                {
                  Id = addressEntity.AddressID,
                  UserId  = addressEntity.CustomerId ,
                  Address = addressEntity.Line1+ addressEntity.Line2 + addressEntity.City + addressEntity.State + addressEntity.PostalCode,
                }
                );
            }
            catch (Exception ex)
            {
                return BaseResponse<AddressResponse>.Failure(
                    ex.GetType().Name , ex.Message);
            }
        }

        public BaseResponse<AddressResponse> DeleteAddress(int id)
        {
            var address = GetAddressById(id);
            var isAdressPresent = address.Data != null && address.IsSuccess == true ;   // to check if address exists
            if(!isAdressPresent)
            {
                return BaseResponse<AddressResponse>.Failure(
                    "Not Found","Address not found or empty");
            }
           var addressEntity = _identityContext.Addresses.Remove(address.Data).Entity;
            _identityContext.SaveChanges();
            return BaseResponse<AddressResponse>.Success
                (
                 new AddressResponse 
                 {
                     Id = addressEntity.AddressID,
                     UserId = addressEntity.CustomerId ,
                     Address = addressEntity.Line1 + addressEntity.Line2 + addressEntity.City + addressEntity.State + addressEntity.PostalCode,
                 }
                );
        }

        public BaseResponse<Address> GetAddressById(int id)
        {
            if (id <= 0)
            {
                return BaseResponse<Address>.Failure("BadRequest" , "Invalid address id");
            }
            var address = _identityContext.Addresses.Where(a => a.AddressID == id).FirstOrDefault();
            return address != null ? BaseResponse<Address>.Success(address) 
                                     : BaseResponse<Address>.Failure("Not Found", "Address does not exists.");

        }

        public BaseResponse<List<Address>> GetAllAddresses(string userID)
        {
            var Addresses = _identityContext.Addresses.Where(x => x.CustomerId == userID).ToList();
            if (Addresses == null || Addresses.Count == 0)
            {
                return BaseResponse<List<Address>>.Failure("Not Found", "No Address Present");
            }
            return BaseResponse<List<Address>>.Success(Addresses);
        }

        public BaseResponse<AddressResponse> UpdateAddress(int id, AddressDto addressDto)
        {
            var addressResponse = GetAddressById(id);
            if (!addressResponse.IsSuccess || addressResponse.Data == null || addressResponse == null)
            {
                var errors = addressResponse.Errors.FirstOrDefault();
                return BaseResponse<AddressResponse>.Failure(errors.Code , errors.Message);
            }
            try
            {
                var AddressEntity = addressResponse.Data;
                AddressEntity.Line1 = addressDto.StreetLine1;
                AddressEntity.Line2 = addressDto.StreetLine2;
                AddressEntity.City = addressDto.City;
                AddressEntity.State = addressDto.State;
                AddressEntity.PostalCode = addressDto.PinCode;

                var addressEntity = _identityContext.Addresses.Update(addressResponse.Data).Entity;
                _identityContext.SaveChanges();
                return BaseResponse<AddressResponse>.Success(
                    new AddressResponse 
                    {
                        Id = addressEntity.AddressID,
                        UserId = addressEntity.CustomerId,
                        Address = addressEntity.Line1 + addressEntity.Line2 + addressEntity.City + addressEntity.State + addressEntity.PostalCode,

                    }
                    );
            }
            catch (Exception ex)
            {
                return BaseResponse<AddressResponse>.Failure(
                     "UnExpected Error","Unexpected error occured while updating address"
                    );
            }
        }
    }
}
