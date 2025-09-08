using AuthServer.Utilities.CustomValidators;
using System.ComponentModel.DataAnnotations;

namespace DataTypeMapping.Dto
{
    public class CustomerDto
    {
        [Required(ErrorMessage ="Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Email cannot be empty")]
        [EmailAddress]
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        [Required(ErrorMessage ="password cannot be empty")]
        [MinLength(10)]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        [AddressRequiredIfAnyFilled]
        public AddressDto Address { get; set; }
    }
}
