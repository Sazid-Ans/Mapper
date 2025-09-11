using Microsoft.EntityFrameworkCore;

namespace OrderService.OrderService.Infrastructure.Persistence
{
    public class OrderServiceDbContext : DbContext
    {
        public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options)  : base(options)
        {
                
        }
    }
}
