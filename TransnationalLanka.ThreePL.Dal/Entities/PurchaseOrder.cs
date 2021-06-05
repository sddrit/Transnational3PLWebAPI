using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        public string Note { get; set; }
        public long SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public long? WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }

    public class PurchaseOrderItem : BaseEntity
    {
        public long PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        [NotMapped]
        public decimal Value => Quantity * Cost;
    }
}
