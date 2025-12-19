using Domain.Enitities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<ClientEntity> Clients { get; set; }
        public DbSet<ProductsEntity> Products { get; set; }
        public DbSet<CartItemsEntity> CartItems { get; set; }
        public DbSet<CartEntity> Carts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Client).WithMany(p => p.Carts)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
            modelBuilder.Entity<CartItemsEntity>(entity =>
            {
                entity.HasKey(e => e.Id);


                entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
