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
    public class MonthlyReconciliationReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public MonthlyReconciliationReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var monthlyReconsilationReportReport = new MonthlyReconsilationReport();

            long warehouseId = 0;

            if (parameters.AllKeys.Contains("warehouseId") && !string.IsNullOrEmpty(parameters["warehouseId"]))
            {
                warehouseId = long.Parse(parameters["warehouseId"]);
            }

            var monthlyReconsilationReportData = AsyncContext.Run(() => _reportService
                .GetMonthlyReconsilationReport(Convert.ToDateTime(parameters["from"]),
                    Convert.ToDateTime(parameters["to"]), warehouseId));

            var monthlyReconsilationReportobjectDataSource = new ObjectDataSource
            {
                DataSource = monthlyReconsilationReportData,
                Name = "monthlyReconcilationDataSource"
            };

            monthlyReconsilationReportReport.DataSource = monthlyReconsilationReportobjectDataSource;

            return monthlyReconsilationReportReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "MonthlyReconciliationReport".ToLower();
        }
    }
}
