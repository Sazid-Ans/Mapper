using DataTypeMapping.Model;

namespace AuthServer.Dto.ResponseDto
{
    public class AddressResponse
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }
    }
}
