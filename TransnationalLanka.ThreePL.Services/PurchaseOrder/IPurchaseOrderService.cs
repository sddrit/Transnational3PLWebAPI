using System.Linq;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.PurchaseOrder
{
    public interface IPurchaseOrderService
    {
        IQueryable<Dal.Entities.PurchaseOrder> GetAll();
        Task<Dal.Entities.PurchaseOrder> AddPurchaseOrder(Dal.Entities.PurchaseOrder purchaseOrder);
        Task<Dal.Entities.PurchaseOrder> UpdatePurchaseOrder(Dal.Entities.PurchaseOrder purchaseOrder);
        Task<Dal.Entities.PurchaseOrder> GetPurchaseOrderById(long id);
    }
}