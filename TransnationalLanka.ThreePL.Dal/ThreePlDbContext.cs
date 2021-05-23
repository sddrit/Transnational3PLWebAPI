using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Dal
{
    public class ThreePlDbContext : IdentityDbContext<User, Role, long>
    {
        public ThreePlDbContext(DbContextOptions<ThreePlDbContext> options): base(options)
        {
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Address>()
                .HasOne(e => e.City)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Supplier>()
                .HasMany(e => e.PickupAddress)
                .WithOne(e => e.Supplier)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);

        }
    }
}
