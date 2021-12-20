namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Account : BaseEntity
    {
        public bool Active { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public long SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
