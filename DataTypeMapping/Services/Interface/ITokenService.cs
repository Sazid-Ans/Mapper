using DataTypeMapping.Dto;

namespace DataTypeMapping.Services.Interface
{
    public interface ITokenService
    {
        void SaveToken(string token, string userId);
        void RevokeToken(string token);
        bool IsTokenRevoked(string token);
    }
}
