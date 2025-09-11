using Microsoft.EntityFrameworkCore;

namespace ProductService.ProductService.Infrastructure.Persistence
{
    public class ProductServiceDbContext : DbContext
    {
        public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options)
            : base(options)
        {

        }
    }
}
