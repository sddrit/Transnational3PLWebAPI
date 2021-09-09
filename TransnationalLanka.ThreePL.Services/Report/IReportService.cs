using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Services.Report.Core;

namespace TransnationalLanka.ThreePL.Services.Report
{
    public interface IReportService
    {
        
        Task<GrnReport> GetGrnReport(long id);
        Task<PurchaseOrderReport> GetPurchaseOrderReport(long id);
        Task<List<WayBill>> GetWayBill(long id);
        Task<InvoiceReport> GetInvoice(long id);
        Task<InventoryReport> GetInventoryReport(long? wareHouseId, long? supplierId);
        Task<InventoryMovementReport> GetInventoryMovementReport(long productId, DateTime fromDate, DateTime toDate,
            long? wareHouseId);
        Task<SellerWiseItemReport> GetSellerWiseItemDetail(long wareHouseId, long supplierId);
        Task<MonthlyReconsilationReport> GetMonthlyReconsilationReport(DateTime fromDate, DateTime toDate, long wareHouseId);
        Task<MonthlySalesReport> GetMonthlySalesReport(DateTime fromDate, DateTime toDate, long wareHouseId);
        Task<SellerWiseItemSummary> GetSellerWiseItemSummary(long wareHouseId);
        Task<StockAdjustmentReport> GetStockAdjustmentReport(long wareHouseId, long supplierId);
        Task<ItemWiseReOrderLevelReport> GetReOrderLevelReport(long? supplierId);
    }
}