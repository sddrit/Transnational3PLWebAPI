namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class ProductStock : BaseEntity
    {
        public long WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public decimal Quantity { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
