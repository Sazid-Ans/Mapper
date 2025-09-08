namespace AuthServer.Dto.ResponseDto
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string JwtToken { get; set; }
    }
}
