using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using DevExpress.DataAccess.ObjectBinding;
using Nito.AsyncEx;
using TransnationalLanka.ThreePL.Services.Report;
using TransnationalLanka.ThreePL.WebApi.Reports;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public class WaybillReportGenerator : IReportGenerator
    {
        private readonly IReportService _reportService;

        public WaybillReportGenerator(IReportService reportService)
        {
            _reportService = reportService;
        }

        public XtraReport GenerateReport(NameValueCollection parameters)
        {
            var wayBillData = AsyncContext.Run(() => _reportService.GetWayBill(long.Parse(parameters["id"].ToString())));

            var report = new WayBill();

            var wayBillObjectDataSource = new ObjectDataSource
            {
                DataSource = wayBillData[0],
                Name = "wayBillDataSource"
            };
            report.DataSource = wayBillObjectDataSource;

            report.CreateDocument(true);

            for (int i = 1; i < wayBillData.Count; i++)
            {
                var wayBillReport = new WayBill();

                wayBillObjectDataSource = new ObjectDataSource
                {
                    DataSource = wayBillData[i],
                    Name = "wayBillDataSource"
                };
                wayBillReport.DataSource = wayBillObjectDataSource;
                wayBillReport.CreateDocument(true);
                report.ModifyDocument(x =>
                {
                    x.AddPages(wayBillReport.Pages);

                });
            }
            return report;
        }

        public bool IsMatch(string key)
        {
            return key.ToLower() == "WayBill".ToLower();
        }
    }
}
