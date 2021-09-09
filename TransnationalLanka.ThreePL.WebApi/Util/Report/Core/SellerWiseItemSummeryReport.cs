using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;
using System.Linq;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class SellerWiseItemSummeryReport : IReportGenerator
    {
        private readonly IReportService _reportService;

        public SellerWiseItemSummeryReport(IReportService reportService)
        {
            _reportService = reportService;
        }


        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            long warehouseId = 0;
            long supplierId = 0;

            if (parameters.AllKeys.Contains("warehouseId") && !string.IsNullOrEmpty(parameters["warehouseId"]))
            {
                warehouseId = long.Parse(parameters["warehouseId"]);
            }

            if (parameters.AllKeys.Contains("supplierId") && !string.IsNullOrEmpty(parameters["supplierId"]))
            {
                supplierId = long.Parse(parameters["supplierId"]);
            }

            var selllerWiseItemDetailReport = new SellerWiseItemDetailReport();

            var selllerWiseItemDetailReportData = AsyncContext.Run(() => _reportService.GetSellerWiseItemDetail(
                warehouseId,
                supplierId));

            var selllerWiseItemDetailObjectDataSource = new ObjectDataSource
            {
                DataSource = selllerWiseItemDetailReportData,
                Name = "SellerWiseItemDetailDataSource"
            };

            selllerWiseItemDetailReport.DataSource = selllerWiseItemDetailObjectDataSource;

            return selllerWiseItemDetailReport;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "sellerwiseitemsummery".ToLower();
        }
    }
}
