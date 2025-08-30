using DataTypeMapping.Dto;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities;
using DataTypeMapping.Utilities.AppSettingsDO;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataTypeMapping.Services
{
    public class JwtService : IJwtService
    {
        private readonly IOptions<JwtSettings> _options;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly ITokenService _tokenService;

        public JwtService(IOptions<JwtSettings> options, ITokenService tokenService)
        {
            _options = options;
            _jwtHandler = new JwtSecurityTokenHandler();
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
            _tokenService = tokenService;
        }

        public BaseResponse<string> GenerateToken(TokenDto tokenDto)
        {
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, tokenDto.UserId),
                new Claim(JwtRegisteredClaimNames.Email, tokenDto.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                //new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(now.AddMinutes(_options.Value.ExpiryInMinutes)).ToUnixTimeSeconds().ToString()),
            };
            
            //Custom claims
            if (tokenDto.Roles != null)
            {
                claims.AddRange(tokenDto.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
            }

            if (tokenDto.CustomClaims != null)
            {
                foreach (var kvp in tokenDto.CustomClaims)
                    claims.Add(new Claim(kvp.Key, kvp.Value));
            }

            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256); //Length = 32 chars → 32 bytes = 256 bits

            var jwt = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.Value.ExpiryInMinutes),
            signingCredentials: creds
            );

            try
            {
                string token = _jwtHandler.WriteToken(jwt);
                if (token != null)
                {
                    _tokenService.SaveTokenInCookies(token);

                    return BaseResponse<string>.Success(token);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating token", ex);
            }
            return BaseResponse<string>.Failure(new List<string> { "Token generation failed" });
        }

        public string? GetClaimFromToken(string token, string claimType)
        {
            var principal = ValidateToken(token); // trusted path
            return principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

            // If you ever only want to read without validating (NOT FOR AUTH):
            // var jwt = _handler.ReadJwtToken(token);
            // return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Value.SecretKey,
                ValidateAudience = true,
                ValidAudience = _options.Value.SecretKey,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Optional: Adjust for clock skew if necessary
            };
            var principal = _jwtHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            // defense-in-depth: ensure token really is a JWT with expected alg
            if (validatedToken is JwtSecurityToken jwt &&
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
            {
                throw new SecurityTokenException("Invalid token algorithm.");
            }
            return principal;
        }
    }
}
