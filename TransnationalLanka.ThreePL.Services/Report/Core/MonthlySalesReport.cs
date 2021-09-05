using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
   
    public class MonthlySalesReportItem
    {
        public DateTimeOffset Date { get; set; }
        public string Code { get; set; }
        public string SalesInvoiceNumber { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value => Quantity * UnitPrice;

    }

    public class MonthlySalesReport
    {

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseAddressLine1 { get; set; }
        public string WareHouseAddressLine2 { get; set; }
     
        public List<MonthlySalesReportItem> MonthlySalesReportItems { get; set; }
    }
}
