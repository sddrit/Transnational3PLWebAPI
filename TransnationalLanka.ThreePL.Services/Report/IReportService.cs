using System;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Services.Report.Core;

namespace TransnationalLanka.ThreePL.Services.Report
{
    public interface IReportService
    {
        Task<InventoryReport> GetInventoryReport(long? wareHouseId, long? supplierId);
        Task<GrnReport> GetGrnReport(long id);
        Task<WayBill> GetWayBill(long id);
        Task<InventoryMovementReport> GetInventoryMovementReport(long? wareHouseId, DateTime fromDate, DateTime toDate, int? productId);        
        Task<InvoiceReport> GetInvoice(long id);
    }
}