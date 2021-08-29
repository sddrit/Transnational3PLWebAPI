using System;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferItem> StockTransferItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryItem> DeliveryItems { get; set; }
        public DbSet<DeliveryHistory> DeliveryHistories { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<DeliveryTracking> DeliveryTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Supplier>()
                .HasIndex(s => s.Code)
                .IsUnique();

            builder.Entity<WareHouse>()
                .HasIndex(w => w.Code)
                .IsUnique();

            builder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            builder.Entity<PurchaseOrder>()
                .Property(p => p.PoNumber)
                .IsUnicode(false)
                .HasComputedColumnSql("('PO'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            builder.Entity<GoodReceivedNote>()
                .Property(p => p.GrnNo)
                .IsUnicode(false)
                .HasComputedColumnSql("('GRN'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            builder.Entity<StockTransfer>()
                .Property(p => p.StockTransferNumber)
                .IsUnicode(false)
                .HasComputedColumnSql("('ST'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            builder.Entity<Delivery>()
                .Property(p => p.DeliveryNo)
                .IsUnicode(false)
                .HasComputedColumnSql("('DL'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            builder.Entity<Invoice>()
                .Property(p => p.InvoiceNo)
                .IsUnicode(false)
                .HasComputedColumnSql("('IN'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

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

            builder.Entity<ProductStockAdjustment>()
                .HasOne(ps => ps.WareHouse)
                .WithMany(w => w.ProductStockAdjustments)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StockTransfer>()
                .HasOne(s => s.ToWareHouse)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StockTransfer>()
                .HasOne(s => s.FromWareHouse)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StockTransferItem>()
                .HasOne(t => t.Product)
                .WithMany(p => p.StockTransferItems)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Delivery>()
                .HasOne(d => d.Supplier)
                .WithMany(s => s.Deliveries)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<DeliveryItem>()
                .HasOne(d => d.Product)
                .WithMany(p => p.DeliveryItems)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);

        }
    }
}
