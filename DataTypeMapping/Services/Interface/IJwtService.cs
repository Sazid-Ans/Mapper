using DataTypeMapping.Dto;
using DataTypeMapping.Utilities;
using System.Security.Claims;

namespace DataTypeMapping.Services.Interface
{
    public interface IJwtService
    {
        BaseResponse<string> GenerateToken(TokenDto tokenDto);
        ClaimsPrincipal ValidateToken(string token);
        string? GetClaimFromToken(string token, string claimType);
    }
}
