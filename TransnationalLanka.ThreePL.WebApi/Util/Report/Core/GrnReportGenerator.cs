using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class GrnReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public GrnReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var grnReport = new Grn();

            var grnReportData = AsyncContext.Run(() => _reportService.GetGrnReport(long.Parse(parameters["id"])));

            var grnObjectDataSource = new ObjectDataSource
            {
                DataSource = grnReportData,
                Name = "GrnDataSource"
            };

            grnReport.DataSource = grnObjectDataSource;

            return grnReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "GrnReport".ToLower();
        }
    }
}
