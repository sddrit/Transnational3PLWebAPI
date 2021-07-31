using System;
using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public interface IStockService
    {
        Task AdjustStock(long warehouseId, long productId, decimal unitPrice, decimal quantity, 
            DateTime? expiredDate, string note);
        Task AdjustReturnStock(long warehouseId, long productId, decimal unitCost, decimal quantity,
            DateTime? expiredDate, string note);
        IQueryable<ProductStockAdjustment> GetStockAdjustmentsByProductId(long id);
        IQueryable<ProductStock> GetStocksByProductId(long id);
    }
}