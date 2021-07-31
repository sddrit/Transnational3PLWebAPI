using System;
using System.Collections.Generic;
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

                    var inventoryReportData = AsyncContext.Run(() => _reportService.GetInventoryReport( null,null));
                    var objectDataSource = new ObjectDataSource
                    {
                        DataSource = inventoryReportData,
                        Name = "InventoryBalanceDataSource"
                    };
                    stockBalanceReport.DataSource = objectDataSource;
                    return stockBalanceReport;

                case "GrnReport":
                    var grnReport = new Grn();
                    var grnReportData = AsyncContext.Run(() => _reportService.GetGrnReport(11));
                    var grnObjectDataSource = new ObjectDataSource
                    {
                        DataSource = grnReportData,
                        Name = "GrnDataSource"
                    };
                    grnReport.DataSource = grnObjectDataSource;
                    return grnReport;

                case "WayBill":
                    var wayBillData = AsyncContext.Run(() => _reportService.GetWayBill(1));

                    var report = new WayBill();
                    var wayBillObjectDataSource = new ObjectDataSource
                    {
                        DataSource = wayBillData,
                        Name = "wayBillDataSource"
                    };

                    report.DataSource = wayBillObjectDataSource;
                    report.Parameters["trackingNo"].Value = wayBillData.TrackingNo[0];                  
                    report.CreateDocument(true);
                    report.PrintingSystem.ContinuousPageNumbering = false;                   

                    for (int i= 1; i<wayBillData.TrackingNo.Length;i++)
                    {
                        var wayBillReport = new WayBill() ;

                        wayBillObjectDataSource = new ObjectDataSource
                        {
                            DataSource = wayBillData,
                            Name = "wayBillDataSource"
                        };
                        wayBillReport.DataSource = wayBillObjectDataSource;
                        wayBillReport.Parameters["trackingNo"].Value = wayBillData.TrackingNo[i];
                        wayBillReport.CreateDocument(true);
                        wayBillReport.PrintingSystem.ContinuousPageNumbering = false;

                        report.ModifyDocument(x =>
                        {
                            x.AddPages(wayBillReport.Pages);

                        });
                    }
                    return report;

                case "StockMovementReport":
                    var stockMovementReport = new StockMovementReport();
                    var stockMovementReportData = AsyncContext.Run(() => _reportService.GetInventoryMovementReport(null,Convert.ToDateTime("01-06-2021"),Convert.ToDateTime( "06-30-2021"),1));

                    stockMovementReport.Parameters["fromDate"].Value = "06-01-2021";
                    stockMovementReport.Parameters["toDate"].Value = "06-30-2021";
                    stockMovementReport.Parameters["itemCode"].Value = "P001";


                    var stockMovementObjectDataSource = new ObjectDataSource
                    {
                        DataSource = stockMovementReportData,
                        Name = "stockMovementDataSource"
                    };
                    stockMovementReport.DataSource = stockMovementObjectDataSource;
                    return stockMovementReport;

                case "Invoice":
                    var invoiceReport = new InvoiceSvat();
                    var invoiceReportData = AsyncContext.Run(() => _reportService.GetInvoice(2));
                    var invoiceObjectDataSource = new ObjectDataSource
                    {
                        DataSource = invoiceReportData,
                        Name = "invoiceDataSource"
                    };
                    invoiceReport.DataSource = invoiceObjectDataSource;
                    return invoiceReport;

            }

            return null;
        }
    }
}

