using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class ItemWiseReOrderLevelReport
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public List<ItemWiseReOrderLevelDetailReport> ItemWiseReOrderLevelDetails { get; set; }
    }

    public class ItemWiseReOrderLevelDetailReport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal ReOrderLevel { get; set; }
        public decimal StockInHand { get; set; }
        public decimal ReOrderQty => ReOrderLevel - StockInHand;

    }
}
