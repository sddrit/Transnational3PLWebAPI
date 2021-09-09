using System;
using System.Collections.Generic;
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class InventoryMovementReport
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
        public List<InventoryMovementReportItem> InventoryMovementReportItems { get; set; }
    }

    public class InventoryMovementReportItem
    {
        public DateTimeOffset Date { get; set; }
        public string TypeName { get; set; }
        public  StockAdjustmentType Type { get; set; }
        public string Note { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }

        public decimal Value
        {
            get
            {
                if (Type == StockAdjustmentType.DamageOut || Type == StockAdjustmentType.DispatchReturnOut ||
                    Type == StockAdjustmentType.Out || Type == StockAdjustmentType.SalesReturnOut)
                {
                    return -1 * Quantity * UnitPrice;
                }

                return Quantity * UnitPrice;
            }
        }
    }
}
