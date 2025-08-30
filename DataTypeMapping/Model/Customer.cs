using Microsoft.AspNetCore.Identity;

namespace DataTypeMapping.Model
{
    public class Customer : IdentityUser
    {
        public List<Address> Addresses { get; set; }  //default convention for FK
    }
}
