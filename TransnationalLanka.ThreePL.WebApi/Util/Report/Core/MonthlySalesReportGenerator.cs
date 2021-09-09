using System;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class MonthlySalesReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public MonthlySalesReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var monthlySalesReport = new MonthlySalesReport();

            long warehouseId = 0;

            if (parameters.AllKeys.Contains("warehouseId") && !string.IsNullOrEmpty(parameters["warehouseId"]))
            {
                warehouseId = long.Parse(parameters["warehouseId"]);
            }

            var monthlySalesReportData = AsyncContext.Run(() => _reportService.GetMonthlySalesReport(
                Convert.ToDateTime(parameters["from"]),
                Convert.ToDateTime(parameters["to"]), warehouseId));

            var monthlySalesReportobjectDataSource = new ObjectDataSource
            {
                DataSource = monthlySalesReportData,
                Name = "monthlySalesDataSource"
            };

            monthlySalesReport.DataSource = monthlySalesReportobjectDataSource;

            return monthlySalesReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "MonthlySalesReport".ToLower();
        }
    }
}
