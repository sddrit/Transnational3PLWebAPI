using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class SellerWiseItemSummaryReport
    {

        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }

        public List<SellerWiseItemReportDetail> SellerWiseItemReportDetails { get; set; }
    }
    public class SellerWiseItemSummaryReportDetail
    {
        public string SupplierCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
    }
}
