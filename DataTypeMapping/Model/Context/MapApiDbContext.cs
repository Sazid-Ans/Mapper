using Microsoft.EntityFrameworkCore;

namespace DataTypeMapping.Model.Context
{
    public class MapApiDbContext : DbContext
    {
        public MapApiDbContext(DbContextOptions<MapApiDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> orderLines { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            // Product → BasePrice
            modelBuilder.Entity<Product>(entity =>
            {
                entity.OwnsOne(p => p.BasePrice, price =>
                {
                    price.Property(m => m.Amount)
                         .HasColumnName("BasePriceAmount")
                         .IsRequired();

                    price.Property(m => m.Currency)
                         .HasColumnName("BasePriceCurrency")
                         .HasMaxLength(10);
                });

                // ignore computed property (EF can’t persist it)
                entity.Ignore(p => p.PriceAfterDiscount);
            });

            // Order → Total (Money)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.OwnsOne(o => o.Total, price =>
                {
                    price.Property(m => m.Amount)
                         .HasColumnName("TotalAmount")
                         .IsRequired();

                    price.Property(m => m.Currency)
                         .HasColumnName("TotalCurrency")
                         .HasMaxLength(10);
                });
            });

            // OrderLine → UnitPrice
            modelBuilder.Entity<OrderLine>(entity =>
            {
                entity.OwnsOne(o => o.UnitPrice, price =>
                {
                    price.Property(m => m.Amount)
                         .HasColumnName("UnitPriceAmount")
                         .IsRequired();

                    price.Property(m => m.Currency)
                         .HasColumnName("UnitPriceCurrency")
                         .HasMaxLength(10);
                });
            });

            // Payment → Total Amount
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.OwnsOne(p=> p.Amount, price =>
                {
                    price.Property(m => m.Amount)
                         .HasColumnName("UnitPriceAmount")
                         .IsRequired();

                    price.Property(m => m.Currency)
                         .HasColumnName("UnitPriceCurrency")
                         .HasMaxLength(10);
                });
            });
        }

    }
}
