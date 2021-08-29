using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Product.Core;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public interface IStockService
    {
        Task AdjustStock(long warehouseId, long productId, decimal unitPrice, decimal quantity, 
            DateTime? expiredDate, string note);
        Task AdjustReturnStock(long warehouseId, long productId, decimal unitCost, decimal quantity,
            DateTime? expiredDate, string note);
        Task TransferReturnStock(long warehouseId, long productId, decimal unitCost, decimal quantity,
            DateTime? expiredDate,
            string note);
        IQueryable<ProductStockAdjustment> GetStockAdjustmentsByProductId(long id);
        IQueryable<ProductStock> GetStocksByProductId(long id);
        Task<decimal> GetTotalStorage(long supplierId);
        Task<decimal> GetRemainStorage(long supplierId);
        Task<decimal> CalculateStorage(CalculateStorageByProducts calculateStorageByProducts);
        Task<List<TotalStorageByWareHouse>> GetTotalStorageByWareHouses(long supplierId);
    }
}