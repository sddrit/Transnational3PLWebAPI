using System;
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
              

                case "GrnReport":
                    var grnReport = new Grn();
                    var grnReportData = AsyncContext.Run(() => _reportService.GetGrnReport(long.Parse(parameters["id"].ToString())));
                    var grnObjectDataSource = new ObjectDataSource
                    {
                        DataSource = grnReportData,
                        Name = "GrnDataSource"
                    };
                    grnReport.DataSource = grnObjectDataSource;
                    return grnReport;

                case "WayBill":
                    var wayBillData = AsyncContext.Run(() => _reportService.GetWayBill(long.Parse(parameters["id"].ToString())));
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

                    for (int i= 1; i<wayBillData.TrackingNo.Length; i++)
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

                   
                case "PurchaseOrder":
                    var purchaseOrderReport = new PurchaseOrderReport ();
                    //var purchaseOrderReportData = AsyncContext.Run(() => _reportService.GetPurchaseOrderReport(long.Parse(parameters["id"].ToString())));
                    var purchaseOrderReportData = AsyncContext.Run(() => _reportService.GetPurchaseOrderReport(2));
                    var purchaseOrderObjectDataSource = new ObjectDataSource
                    {
                        DataSource = purchaseOrderReportData,
                        Name = "purchaseOrderDataSource"
                    };
                    purchaseOrderReport.DataSource = purchaseOrderObjectDataSource;
                    return purchaseOrderReport;

               

                case "Invoice":
                    var invoiceReport = new InvoiceSvat();
                    var invoiceReportData = AsyncContext.Run(() => _reportService.GetInvoice(long.Parse(parameters["id"].ToString())));
                    var invoiceObjectDataSource = new ObjectDataSource
                    {
                        DataSource = invoiceReportData,
                        Name = "invoiceDataSource"
                    };
                    invoiceReport.DataSource = invoiceObjectDataSource;
                    return invoiceReport;

                case "SellerWiseItemDeailReport":
                    var selllerWiseItemDetailReport = new SellerWiseItemDetailReport();
                    //var selllerWiseItemDetailReportData = AsyncContext.Run(() => _reportService.GetSellerWiseItemDetail(
                    //    long.Parse(parameters["wareHouseid"].ToString()),
                    //    long.Parse(parameters["supplierid"].ToString())));
                    var selllerWiseItemDetailReportData = AsyncContext.Run(() => _reportService.GetSellerWiseItemDetail(
                     1,
                     3));

                    var selllerWiseItemDetailObjectDataSource = new ObjectDataSource
                    {
                        DataSource = selllerWiseItemDetailReportData,
                        Name = "SellerWiseItemDetailDataSource"
                    };
                    selllerWiseItemDetailReport.DataSource = selllerWiseItemDetailObjectDataSource;
                    return selllerWiseItemDetailReport;

                case "StockMovementReport":
                    var stockMovementReport = new StockBalanceReport();
                    var stockMovementReportData = AsyncContext.Run(() => _reportService.GetInventoryMovementReport(null, Convert.ToDateTime(parameters["fromDate"].ToString()), Convert.ToDateTime(parameters["toDate"].ToString()), long.Parse(parameters["productId"].ToString())));
                    var stockMovementObjectDataSource = new ObjectDataSource
                    {
                        DataSource = stockMovementReportData,
                        Name = "stockMovementDataSource"
                    };
                    stockMovementReport.DataSource = stockMovementObjectDataSource;
                    return stockMovementReport;

                case "StockBalanceReport":
                    var stockBalanceReport = new StockBalanceReport();
                    var inventoryReportData = AsyncContext.Run(() => _reportService.GetInventoryReport(null, null));
                    var objectDataSource = new ObjectDataSource
                    {
                        DataSource = inventoryReportData,
                        Name = "InventoryBalanceDataSource"
                    };
                    stockBalanceReport.DataSource = objectDataSource;
                    return stockBalanceReport;
            }

            return null;
        }
    }
}

