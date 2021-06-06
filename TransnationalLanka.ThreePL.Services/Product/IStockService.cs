using System;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public interface IStockService
    {
        Task AdjustStock(long warehouseId, long productId, decimal unitPrice, decimal quantity, DateTime? expiredDate, string note);
    }
}