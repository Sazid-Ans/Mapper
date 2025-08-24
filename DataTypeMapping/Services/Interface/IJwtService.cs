using DataTypeMapping.Dto;
using System.Security.Claims;

namespace DataTypeMapping.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(TokenDto tokenDto);
        ClaimsPrincipal ValidateToken(string token);
        string? GetClaimFromToken(string token, string claimType);
    }
}
