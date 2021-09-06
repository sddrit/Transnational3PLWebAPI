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
                //to be pass parameters----------
                case "PurchaseOrder":

                    var purchaseOrderReport = new PurchaseOrderReport();

                    var purchaseOrderReportData = AsyncContext.Run(() => _reportService.GetPurchaseOrderReport(long.Parse(parameters["id"].ToString())));

                    var purchaseOrderObjectDataSource = new ObjectDataSource
                    {
                        DataSource = purchaseOrderReportData,
                        Name = "purchaseOrderDataSource"
                    };

                    purchaseOrderReport.DataSource = purchaseOrderObjectDataSource;

                    return purchaseOrderReport;

                case "SellerWiseItemDeailReport":

                    var selllerWiseItemDetailReport = new SellerWiseItemDetailReport();

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

                case "MonthlyReconsilationReport":
                    var monthlyReconsilationReportReport = new MonthlyReconsilationReport();

                    var monthlyReconsilationReportData = AsyncContext.Run(() => _reportService.GetMonthlyReconsilationReport(new DateTime(2021, 08, 01), new DateTime(2021, 08, 31), 1));

                    var monthlyReconsilationReportobjectDataSource = new ObjectDataSource
                    {
                        DataSource = monthlyReconsilationReportData,
                        Name = "monthlyReconcilationDataSource"
                    };

                    monthlyReconsilationReportReport.DataSource = monthlyReconsilationReportobjectDataSource;

                    return monthlyReconsilationReportReport;

                case "MonthlySalesReport":
                    var monthlySalesReport = new MonthlySalesReport();

                    var monthlySalesReportData = AsyncContext.Run(() => _reportService.GetMonthlySalesReport(new DateTime(2021, 08, 01), new DateTime(2021, 09, 30), 1));

                    var monthlySalesReportobjectDataSource = new ObjectDataSource
                    {
                        DataSource = monthlySalesReportData,
                        Name = "monthlySalesDataSource"
                    };

                    monthlySalesReport.DataSource = monthlySalesReportobjectDataSource;

                    return monthlySalesReport;

                case "SellerWiseItemSummaryReport":

                    var sellerWiseItemSummaryReport = new SellerWiseItemSummaryReport();

                    var sellerWiseItemSummaryReportData = AsyncContext.Run(() => _reportService.GetSellerWiseItemSummary(1));

                    var sellerWiseItemSummaryReportobjectDataSource = new ObjectDataSource
                    {
                        DataSource = sellerWiseItemSummaryReportData,
                        Name = "sellerWiseItemSummaryDataSource"
                    };

                    sellerWiseItemSummaryReport.DataSource = sellerWiseItemSummaryReportobjectDataSource;

                    return sellerWiseItemSummaryReport;


                case "StockAdjustmentReport":

                    var stockAdjustmentReport = new StockAdjustmentReport();

                    var stockAdjustmentReportData = AsyncContext.Run(() => _reportService.GetStockAdjustmentReport(1,1));

                    var stockAdjustmentReportobjectDataSource = new ObjectDataSource
                    {
                        DataSource = stockAdjustmentReportData,
                        Name = "stockAdjustmentDataSource"
                    };

                    stockAdjustmentReport.DataSource = stockAdjustmentReportobjectDataSource;

                    return stockAdjustmentReport;


                case "ItemWiseReOrderLevel":

                    var itemWiseReOrderLevel = new ItemWiseReOrderLevelReport();

                    var itemWiseReOrderLevelData = AsyncContext.Run(() => _reportService.GetReOrderLevelReport(1));

                    var itemWiseReOrderLevelobjectDataSource = new ObjectDataSource
                    {
                        DataSource = itemWiseReOrderLevelData,
                        Name = "itemWiseReOrderLevelDataSource"
                    };

                    itemWiseReOrderLevel.DataSource = itemWiseReOrderLevelobjectDataSource;

                    return itemWiseReOrderLevel;
            }
            //to be pass parameters----------
            return null;
        }
    }
}

