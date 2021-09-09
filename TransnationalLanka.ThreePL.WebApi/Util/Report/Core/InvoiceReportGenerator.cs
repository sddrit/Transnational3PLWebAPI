using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class InvoiceReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public InvoiceReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var invoiceReport = new InvoiceSvat();

            var invoiceReportData = AsyncContext.Run(() => _reportService.GetInvoice(long.Parse(parameters["id"])));

            var invoiceObjectDataSource = new ObjectDataSource
            {
                DataSource = invoiceReportData,
                Name = "invoiceDataSource"
            };

            invoiceReport.DataSource = invoiceObjectDataSource;

            return invoiceReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "Invoice".ToLower();
        }
    }
}
