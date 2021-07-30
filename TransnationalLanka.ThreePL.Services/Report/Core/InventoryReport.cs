using System;
using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class InventoryReport
    {
        public DateTime Date { get; set; }
        public string WareHouse { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public List<InventoryReportItem> InventoryReportItems { get; set; }
    }

    public class InventoryReportItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value => Quantity * UnitPrice;
    }
}
