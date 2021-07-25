using System;
using System.Collections.Generic;
using TransnationalLanka.ThreePL.WebApi.Models.Product;
using TransnationalLanka.ThreePL.WebApi.Models.WareHouse;

namespace TransnationalLanka.ThreePL.WebApi.Models.StockTransfer
{
    public class StockTransferBindingModel
    {
        public long Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public long ToWareHouseId { get; set; }
        public WareHouseBindingModel ToWareHouse { get; set; }
        public long FromWareHouseId { get; set; }
        public WareHouseBindingModel FromWareHouse { get; set; }
        public string Reason { get; set; }
        public virtual ICollection<StockTransferItemBindingModel> StockTransferItems { get; set; }
    }

    public class StockTransferItemBindingModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public virtual ProductBindingModel Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public decimal Value => Quantity * UnitCost;
    }
}
