using DataTypeMapping.Dto;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using System.Security.AccessControl;

namespace DataTypeMapping.Services
{
    public class AddressService : IAddressService
    {
        private readonly MapApiIdentityContext _identityContext;

        public AddressService(MapApiIdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        public BaseResponse<Address> CreateAddress(AddressDto addressDto)
        {
            if (addressDto  == null) 
            {
                var error = new List<string> { "Address data is null" };
                return BaseResponse<Address>.Failure(error);
            }
            var address = new Address
            {
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

        public BaseResponse<Address> DeleteAddress(int id)
        {
            var addressResponse = GetAddressById(id);
            var isAdressPresent = addressResponse.Data != null && addressResponse.IsSuccess == true ;   // to check if address exists
            if(!isAdressPresent)
            {
                return BaseResponse<Address>.Failure(new List<string> { "Address not found" });
            }
            _identityContext.Addresses.Remove(addressResponse.Data);
            _identityContext.SaveChanges();
            return BaseResponse<Address>.Success(null);
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

        public BaseResponse<List<Address>> GetAllAddresses()
        {
            var Addresses = _identityContext.Addresses.Select(x => x).ToList();
            if (Addresses == null || Addresses.Count == 0)
            {
                return BaseResponse<List<Address>>.Failure(new List<string> { "Address count zero" });
            }
            return BaseResponse<List<Address>>.Success(Addresses);
        }   

        public BaseResponse<Address> UpdateAddress(int id, AddressDto addressDto)
        {
            var addressResponse = GetAddressById(id);
            if (!addressResponse.IsSuccess || addressResponse.Data == null || addressDto == null)
            {
                return BaseResponse<Address>.Failure(new List<string> { "Address or Address data not found" });
            }
            var address = addressResponse.Data;
            address.Line1 = addressDto.StreetLine1;
            address.Line2 = addressDto.StreetLine2;
            address.City = addressDto.City;
            address.PostalCode = addressDto.PinCode;
            address.State = addressDto.State;
            _identityContext.Addresses.Update(address);
            _identityContext.SaveChanges();
           return BaseResponse<Address>.Success(address);
        }
    }
}
