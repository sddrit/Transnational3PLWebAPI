using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Report.Core;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.Services.WareHouse;

namespace TransnationalLanka.ThreePL.Services.Report
{
    public interface IReportService
    {
        Task<InventoryReport> GetInventoryReport(long? wareHouseId, long? supplierId);
    }

    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupplierService _supplierService;
        private readonly IWareHouseService _wareHouseService;

        public ReportService(IUnitOfWork unitOfWork, ISupplierService supplierService, IWareHouseService wareHouseService)
        {
            _unitOfWork = unitOfWork;
            _supplierService = supplierService;
            _wareHouseService = wareHouseService;
        }

        public async Task<InventoryReport> GetInventoryReport(long? wareHouseId, long? supplierId)
        {
            string supplierName = null;
            string wareHouseName = null;

            if (supplierId.HasValue)
            {
                var supplier = await _supplierService.GetSupplierById(supplierId.Value);
                supplierName = supplier.SupplierName;
            }

            if (wareHouseId.HasValue)
            {
                var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId.Value);
                wareHouseName = wareHouse.Name;
            }

            var query = _unitOfWork.ProductRepository.GetAll()
                .Where(p => p.Active && p.Supplier.Active)
                .Include(p => p.Stocks)
                .AsQueryable();

            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            if (wareHouseId.HasValue)
            {
                query = query.Where(p => p.Stocks.Any(s => s.WareHouseId == wareHouseId.Value));
            }

            return new InventoryReport()
            {
                SupplierName = supplierName,
                WareHouse = wareHouseName,
                InventoryReportItems = await query.Select(p => new InventoryReportItem()
                {
                    Code = p.Code,
                    UnitPrice = p.UnitPrice,
                    Quantity = p.Stocks.Sum(s => s.Quantity),
                    Description = p.Description
                }).ToListAsync()
            };
        }
    }
}
