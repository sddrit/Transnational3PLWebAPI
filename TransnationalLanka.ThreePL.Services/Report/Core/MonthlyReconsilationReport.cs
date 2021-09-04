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
        public decimal TotalInventory { get; set; }
        public decimal TotalSales { get; set; }
        public decimal DamagedGoods { get; set; }
        public decimal ReturnsFromCustomer { get; set; }
        public decimal ClosingBalance { get; set; }

        public string WareHouse { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
    }
}
