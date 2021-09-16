using System.Collections.Generic;
using System.Threading.Tasks;
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
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferItem> StockTransferItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryItem> DeliveryItems { get; set; }
        public DbSet<DeliveryHistory> DeliveryHistories { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<DeliveryTracking> DeliveryTrackings { get; set; }
        public DbSet<UserWareHouse> UserWareHouses { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Audit> AuditLogs { get; set; }
        public DbSet<Event> Events { get; set; }

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
                .HasComputedColumnSql("([dbo].[FN_GENERATE_PO_NUMBER]([Id]))");

            builder.Entity<GoodReceivedNote>()
                .Property(p => p.GrnNo)
                .IsUnicode(false)
                .HasComputedColumnSql("([dbo].[FN_GENERATE_GRN_NUMBER]([Id]))");

            builder.Entity<StockTransfer>()
                .Property(p => p.StockTransferNumber)
                .IsUnicode(false)
                .HasComputedColumnSql("([dbo].[FN_GENERATE_ST_NUMBER]([Id]))");

            builder.Entity<Delivery>()
                .Property(p => p.DeliveryNo)
                .IsUnicode(false)
                .HasComputedColumnSql("([dbo].[FN_GENERATE_DELIVERY_NUMBER]([Id]))");

            builder.Entity<Invoice>()
                .Property(p => p.InvoiceNo)
                .IsUnicode(false)
                .HasComputedColumnSql("([dbo].[FN_GENERATE_INVOICE_NUMBER]([Id]))");

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

        public virtual async Task<int> SaveChangesAsync(long? userId = null, string userName = null, string machineName = null)
        {
            OnBeforeSaveChanges(userId, userName, machineName);
            var result = await base.SaveChangesAsync();
            return result;
        }

        private void OnBeforeSaveChanges(long? userId = null, string userName = null, string machineName = null)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = userId,
                    UserName = userName,
                    MachineName = machineName
                };
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }
    }
}
