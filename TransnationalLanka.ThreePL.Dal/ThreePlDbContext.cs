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
        public DbSet<WareHouse> WareHouses { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<ProductStockAdjustment> ProductStockAdjustments { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<GoodReceivedNote> GoodReceivedNotes { get; set; }
        public DbSet<GoodReceivedNoteItems> GoodReceivedNoteItems { get; set; }

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

            builder.Entity<ProductStock>()
                .HasOne(s => s.WareHouse)
                .WithMany(w => w.ProductStocks)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<GoodReceivedNote>()
                .HasOne(g => g.WareHouse)
                .WithMany(w => w.GoodReceivedNotes)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PurchaseOrder>()
                .HasMany(p => p.PurchaseOrderItems)
                .WithOne(i => i.PurchaseOrder)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GoodReceivedNoteItems>()
                .HasOne(i => i.Product)
                .WithMany(p => p.GoodReceivedNoteItems)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PurchaseOrderItem>()
                .HasOne(p => p.Product)
                .WithMany(p => p.PurchaseOrderItems)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);

        }
    }
}
