namespace AuthServer.Dto.ResponseDto
{
    public class RegistrationResponse
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
    }
}
