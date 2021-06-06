using System;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public enum StockAdjustmentType
    {
        In = 1,
        Out = 2
    }

    public class ProductStockAdjustment : BaseEntity
    {
        public long ProductId  { get; set; }
        public virtual Product Product { get; set; }
        public StockAdjustmentType Type { get; set; }
        public long WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string Note { get; set; }
    }
}
