using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class SellerWiseItemReport
    {     
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public List<SellerWiseItemReportDetail> SellerWiseItemReportDetails { get; set; }
    }
    public class SellerWiseItemReportDetail
    {
        public string Code { get; set; }
        public string Description { get; set; }      
        public string UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
    }
}
