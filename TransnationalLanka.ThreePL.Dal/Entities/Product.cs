
using System.Collections.Generic;
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public long SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StoringType? StoringType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReorderLevel { get; set; }
        public string Sku { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }

        public decimal StorageUnits => Height * Width * Length;

        public decimal? Weight { get; set; }
        public MassUnit? MassUnit { get; set; }
        public bool Active { get; set; }
        public long? UnitOfMeasureId { get; set; }
        public virtual UnitOfMeasure UnitOfMeasure { get; set; }
        public virtual ICollection<ProductStock> Stocks { get; set; }
        public virtual ICollection<ProductStockAdjustment> StockAdjustments { get; set; }
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public virtual ICollection<GoodReceivedNoteItems> GoodReceivedNoteItems { get; set; }
        public virtual ICollection<StockTransferItem> StockTransferItems { get; set; }
        public virtual ICollection<DeliveryItem> DeliveryItems { get; set; }
    }
}
