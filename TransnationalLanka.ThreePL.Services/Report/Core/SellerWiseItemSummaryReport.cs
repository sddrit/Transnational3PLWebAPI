using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class SellerWiseItemSummary
    {

        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }

        public List<SellerWiseItemSummaryDetail> SellerWiseItemReportDetails { get; set; }
    }
    public class SellerWiseItemSummaryDetail
    {
        public string SupplierCode { get; set; }       
        public string SupplierName { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
    }
}
