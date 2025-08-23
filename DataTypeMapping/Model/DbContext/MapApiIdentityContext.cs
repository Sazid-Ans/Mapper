using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataTypeMapping.Model.DbContext
{
    public class MapApiIdentityContext : IdentityDbContext<Customer>
    {
        public MapApiIdentityContext(DbContextOptions<MapApiIdentityContext> options) : base(options)
        {
           
        }
    }
}

