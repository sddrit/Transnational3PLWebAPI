using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class GoodReceivedNote : BaseEntity
    {
        public string GrnNo { get; set; }
        public long? PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public long SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public long WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public virtual ICollection<GoodReceivedNoteItems> GoodReceivedNoteItems { get; set; }
    }

    public class GoodReceivedNoteItems : BaseEntity
    {
        public long GoodReceivedNoteId { get; set; }
        public virtual GoodReceivedNote GoodReceivedNote { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }
        [NotMapped]
        public decimal Value => Quantity * UnitCost;
    }
}
