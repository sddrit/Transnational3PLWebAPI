using System.Linq;
using System.Web;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Util.Report.Core;

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
            IReportGenerator[] reportGenerators = 
            {
                new StockBalanceReportGenerator(_reportService),
                new InvoiceReportGenerator(_reportService),
                new PurchaseOrderReportGenerator(_reportService),
                new GrnReportGenerator(_reportService),
                new StockMovementReportGenerator(_reportService),
                new MonthlyReconciliationReportGenerator(_reportService),
                new MonthlySalesReportGenerator(_reportService),
                new SellerWiseItemSummeryReport(_reportService),
                new StockReorderReportGenerator(_reportService)
            };

            string[] parts = url.Split("?");
            string reportName = parts[0];
            string parametersString = parts.Length > 1 ? parts[1] : string.Empty;

            var parameters = HttpUtility.ParseQueryString(parametersString);

            if (string.IsNullOrEmpty(reportName))
                return null;

            return (from reportGenerator in reportGenerators where reportGenerator.IsMatch(reportName) 
                select reportGenerator.GenerateReport(parameters)).FirstOrDefault();
        }
    }
}

