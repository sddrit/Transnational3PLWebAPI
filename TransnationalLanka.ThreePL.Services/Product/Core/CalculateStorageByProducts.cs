using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Services.Product.Core
{
    public class CalculateStorageByProducts
    {
        public List<CalculateStorageProductItem> Products { get; set; }
    }

    public class CalculateStorageProductItem
    {
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
