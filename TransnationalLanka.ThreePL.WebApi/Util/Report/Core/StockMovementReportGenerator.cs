using System;
using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class StockMovementReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public StockMovementReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var stockMovementReport = new StockMovementReport();

            long? warehouseId = null;

            if (parameters.AllKeys.Contains("warehouseId") && !string.IsNullOrEmpty(parameters["warehouseId"]))
            {
                warehouseId = long.Parse(parameters["warehouseId"]);
            }

            var stockMovementReportData = AsyncContext.Run(() => _reportService.GetInventoryMovementReport(
                long.Parse(parameters["productId"]), 
                Convert.ToDateTime(parameters["from"]), 
                Convert.ToDateTime(parameters["to"]), 
                warehouseId));

            var stockMovementObjectDataSource = new ObjectDataSource
            {
                DataSource = stockMovementReportData,
                Name = "stockMovementDataSource"
            };

            stockMovementReport.DataSource = stockMovementObjectDataSource;

            return stockMovementReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "StockMovementReport".ToLower();
        }
    }
}
