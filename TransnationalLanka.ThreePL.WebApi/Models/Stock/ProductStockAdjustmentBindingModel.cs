using System;
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.WebApi.Models.Stock
{
    public class ProductStockAdjustmentBindingModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public StockAdjustmentType Type { get; set; }
        public long WareHouseId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string Note { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
    }

    public class ProductStockBindingModel
    {
        public long Id { get; set; }
        public long WareHouseId { get; set; }
        public decimal Quantity { get; set; }
        public decimal DispatchReturnQuantity { get; set; }
        public decimal DamageStockQuantity { get; set; }
        public decimal SalesReturnQuantity { get; set; }
        public long ProductId { get; set; }
    }
}
