using TransnationalLanka.ThreePL.Dal.Core;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class ProductStock : BaseEntity, IWareHouseRelatedEntity
    {
        public long WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public decimal Quantity { get; set; }
        public decimal DispatchReturnQuantity { get; set; }
        public decimal DamageStockQuantity { get; set; }
        public decimal SalesReturnQuantity { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
