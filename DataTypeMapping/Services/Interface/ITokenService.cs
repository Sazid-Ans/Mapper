namespace DataTypeMapping.Services.Interface
{
    public interface ITokenService
    {
        void SaveTokenInCookies(string token);
        string? GetTokenFromCookies();


        //void RevokeToken(string token);
        //bool IsTokenRevoked(string token);
    }
}
