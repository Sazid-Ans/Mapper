using DataTypeMapping.Model;
using DataTypeMapping.Model.Enum;
using System.ComponentModel.DataAnnotations;

namespace DataTypeMapping.Dto
{
    public class CustomerDto
    {
        [Required(ErrorMessage ="Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Email cannot be empty")]
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        [Required(ErrorMessage ="password cannot be empty")]
        [MinLength(12)]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public AddressDto Address { get; set; }

    }
}
