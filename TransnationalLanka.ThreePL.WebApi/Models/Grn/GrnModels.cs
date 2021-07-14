using System;
using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.WebApi.Models.Grn
{
    public class GoodReceivedNoteBindingModel
    {
        public long Id { get; set; }
        public string GrnNo { get; set; }
        public DateTimeOffset Created { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long SupplierId { get; set; }
        public long WareHouseId { get; set; }
        public virtual ICollection<GoodReceivedNoteItemsBindingModel> GoodReceivedNoteItems { get; set; }
    }

    public class GoodReceivedNoteItemsBindingModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }

}
