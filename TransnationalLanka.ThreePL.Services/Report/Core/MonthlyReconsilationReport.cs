using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class MonthlyReconsilationReport
    {
        public DateTimeOffset ReconsilationDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalReceivedGRN { get; set; }
        public decimal TotalInventory => OpeningBalance + TotalReceivedGRN;
  
        public decimal TotalSales { get; set; }
        public decimal DamagedGoods { get; set; }
        public decimal ReturnsFromCustomer { get; set; }
        public decimal ClosingBalance => TotalInventory + TotalSales+ DamagedGoods+ ReturnsFromCustomer;

        public string WareHouse { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
    }
}
