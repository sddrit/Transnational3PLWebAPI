using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;
using System.Linq;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class StockReorderReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public StockReorderReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            long? supplierId = null;

            if (parameters.AllKeys.Contains("supplierId") && !string.IsNullOrEmpty(parameters["supplierId"]))
            {
                supplierId = long.Parse(parameters["supplierId"]);
            }

            var itemWiseReOrderLevel = new ItemWiseReOrderLevelReport();

            var itemWiseReOrderLevelData = AsyncContext.Run(() => _reportService.GetReOrderLevelReport(supplierId));

            var itemWiseReOrderLevelobjectDataSource = new ObjectDataSource
            {
                DataSource = itemWiseReOrderLevelData,
                Name = "itemWiseReOrderLevelDataSource"
            };

            itemWiseReOrderLevel.DataSource = itemWiseReOrderLevelobjectDataSource;

            return itemWiseReOrderLevel;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "StockReorderReport".ToLower();
        }
    }
}
