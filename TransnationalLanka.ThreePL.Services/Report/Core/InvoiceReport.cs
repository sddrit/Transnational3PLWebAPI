using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class InvoiceReport
    {
        public long InvoiceId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string InvoiceNo { get; set; }
        public long SupplierId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetTotal { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddressLine1 { get; set; }
        public string SupplierAddressLine2 { get; set; }
        public string SupplierVatNumber { get; set; }
        public string SupplierSVatNumber { get; set; }

        public List<InvoiceReportItem> InvoiceReportItems { get; set; }

    }

    public class InvoiceReportItem
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
