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
        public StockAdjustmentType Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime ExpiredDate { get; set; }
        public long? GoodReceivedNoteId { get; set; }
        public virtual GoodReceivedNote GoodReceivedNote { get; set; }
    }
}
