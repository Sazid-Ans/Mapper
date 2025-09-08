using DataTypeMapping.Services.Interface;

namespace DataTypeMapping.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void SaveTokenInCookies(string token)
        {
            // Logic to save the token in cache for 30 minutes
            _httpContextAccessor.HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });
        }
        public string? GetTokenFromCookies()
        {
            // Logic to retrieve the token from cache
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("AuthToken", out var token);
            return token;
        }
    }
}
