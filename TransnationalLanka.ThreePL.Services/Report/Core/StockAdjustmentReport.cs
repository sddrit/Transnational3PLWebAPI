using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class StockAdjustmentReport
    {
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public List<StockAdjustmentItemDetail> StockAdjustmentItemDetails { get; set; }


    }
    public class StockAdjustmentItemDetail
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value => Quantity * UnitPrice;
        public string Remark { get; set; }
    }
}
