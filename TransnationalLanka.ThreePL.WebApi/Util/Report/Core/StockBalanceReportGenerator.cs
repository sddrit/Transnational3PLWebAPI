using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class StockBalanceReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public StockBalanceReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var stockBalanceReport = new Reports.StockBalanceReport();

            long? warehouseId = null;
            long? supplierId = null;

            if (parameters.AllKeys.Contains("warehouseId") && !string.IsNullOrEmpty(parameters["warehouseId"]))
            {
                warehouseId = long.Parse(parameters["warehouseId"]);
            }

            if (parameters.AllKeys.Contains("supplierId") && !string.IsNullOrEmpty(parameters["supplierId"]))
            {
                supplierId = long.Parse(parameters["supplierId"]);
            }

            var inventoryReportData = AsyncContext.Run(() => _reportService.GetInventoryReport(warehouseId, supplierId));

            var dataSource = new ObjectDataSource
            {
                DataSource = inventoryReportData,
                Name = "inventoryBalanceDataSource"
            };

            stockBalanceReport.DataSource = dataSource;

            return stockBalanceReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "StockBalanceReport".ToLower();
        }
    }
}
