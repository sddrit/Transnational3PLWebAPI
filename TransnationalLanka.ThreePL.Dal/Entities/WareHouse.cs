using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class WareHouse : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public WareHouseAddress Address { get; set; }
        public bool Active { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal StorageCapacity => Height * Width * Length;
        public ICollection<ProductStock> ProductStocks { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<GoodReceivedNote> GoodReceivedNotes { get; set; }
        public ICollection<ProductStockAdjustment> ProductStockAdjustments { get; set; }

    }

    [Owned]
    public class WareHouseAddress
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public virtual City City { get; set; }
        public string PostalCode { get; set; }

        public override string ToString()
        {
            var addressLines = new[] { AddressLine1, AddressLine2, City?.CityName };
            return string.Join(", ", addressLines.Where(l => !string.IsNullOrEmpty(l)).ToArray());
        }
    }
}
