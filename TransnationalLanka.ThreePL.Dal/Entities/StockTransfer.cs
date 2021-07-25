using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class StockTransfer : BaseEntity
    {
        public long ToWareHouseId { get; set; }
        public WareHouse ToWareHouse { get; set; }
        public long FromWareHouseId { get; set; }
        public WareHouse FromWareHouse { get; set; }
        public string Reason { get; set; }
        public virtual ICollection<StockTransferItem> StockTransferItems { get; set; }
    }

    public class StockTransferItem : BaseEntity
    {
        public long StockTransferId { get; set; }
        public virtual StockTransfer StockTransfer { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }
        [NotMapped]
        public decimal Value => Quantity * UnitCost;
    }
}
