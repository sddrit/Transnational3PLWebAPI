
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Product : BaseEntity
    {
        public long SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StoringType? StoringType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReorderLevel { get; set; }
        public string Sku { get; set; }
        public decimal StorageUnits { get; set; }
        public decimal? Weight { get; set; }
        public MassUnit? MassUnit { get; set; }
    }
}
