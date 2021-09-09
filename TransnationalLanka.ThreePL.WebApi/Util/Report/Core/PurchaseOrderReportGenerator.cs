using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class PurchaseOrderReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public PurchaseOrderReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var purchaseOrderReport = new PurchaseOrderReport();

            var purchaseOrderReportData = AsyncContext.Run(() => _reportService.GetPurchaseOrderReport(long.Parse(parameters["id"])));

            var purchaseOrderObjectDataSource = new ObjectDataSource
            {
                DataSource = purchaseOrderReportData,
                Name = "purchaseOrderDataSource"
            };

            purchaseOrderReport.DataSource = purchaseOrderObjectDataSource;

            return purchaseOrderReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "PurchaseOrder".ToLower();
        }
    }
}
