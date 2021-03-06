using System;
using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class GrnReport
    {
        public DateTimeOffset Date { get; set; }
        public string WareHouse { get; set; }
        public string GrnType { get; set; }
        public string WareHouseName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string GrnNo { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public string SupplierInvoiceNo { get; set; }
        public List<GrnReportItem> GrnReportItems { get; set; }

    }

    public class GrnReportItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }

    }
}
