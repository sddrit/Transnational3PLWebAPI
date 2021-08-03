using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Stock
{
    public interface IStockTransferService
    {
        IQueryable<Dal.Entities.StockTransfer> GetAll();
        Task<StockTransfer> AddStockTransfer(StockTransfer stockTransfer);
        Task<StockTransfer> GetStockTransferById(long id);
    }
}