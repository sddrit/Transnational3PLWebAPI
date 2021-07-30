using System;
using System.Web;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report
{
    public class ThreePlReportProvider : IReportProvider
    {
        private readonly IReportService _reportService;

        public ThreePlReportProvider(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GetReport(string url, ReportProviderContext context)
        {
            string[] parts = url.Split("?");
            string reportName = parts[0];
            string parametersString = parts.Length > 1 ? parts[1] : String.Empty;

            var parameters = HttpUtility.ParseQueryString(parametersString);

            if (string.IsNullOrEmpty(reportName))
                return null;
            switch (reportName)
            {
                case "StockBalanceReport":
                    var stockBalanceReport = new StockBalanceReport();
                    var inventoryReportData = AsyncContext.Run(() => _reportService.GetInventoryReport(null, 3));
                    var objectDataSource = new ObjectDataSource
                    {
                        DataSource = inventoryReportData,
                        Name = "InventoryBalanceDataSource"
                    };
                    stockBalanceReport.DataSource = objectDataSource;
                    return stockBalanceReport;
            }

            return null;
        }
    }
}

