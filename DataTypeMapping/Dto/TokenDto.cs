namespace DataTypeMapping.Dto
{
    public class TokenDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
        public IDictionary<string, string> CustomClaims { get; set; }
    }
}
