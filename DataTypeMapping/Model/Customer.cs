using DataTypeMapping.Model.Enum;
using System.ComponentModel.DataAnnotations;

namespace DataTypeMapping.Model
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }  //default convention for FK
        public List<Order> Orders{ get; set; }

    }
}
