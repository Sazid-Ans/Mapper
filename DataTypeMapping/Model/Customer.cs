using DataTypeMapping.Model.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DataTypeMapping.Model
{
    public class Customer : IdentityUser
    {
        public List<Address> Addresses { get; set; }  //default convention for FK
    }
}
