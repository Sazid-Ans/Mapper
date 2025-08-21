namespace DataTypeMapping.Dto
{
    public static class RoleDto
    {
        public const string Customer = "Customer";
        public const string Admin = "Admin";
        public const string Vendor = "Vendor";

        public static readonly List<string> Roles = new List<string> { Customer, Admin, Vendor }; 
    }
}
