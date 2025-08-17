using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataTypeMapping.Model
{
    public class MapApiIdentityContext : IdentityDbContext
    {
        public MapApiIdentityContext(DbContextOptions<MapApiIdentityContext> options) : base(options)
        {
           
        }
    }
}

