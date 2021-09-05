using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class ItemWiseReOrderLevelReport
    {
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
        public List<ItemWiseReOrderLevelDetailReport> ItemWiseReOrderLevelDetails { get; set; }
    }

    public class ItemWiseReOrderLevelDetailReport
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal ReOrderLevel { get; set; }
        public decimal StockInHand { get; set; }
        public decimal ReOrderQty => ReOrderLevel - StockInHand;

    }
}
