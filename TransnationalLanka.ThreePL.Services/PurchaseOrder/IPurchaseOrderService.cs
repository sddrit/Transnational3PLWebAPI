using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.Services.PurchaseOrder
{
    public interface IPurchaseOrderService
    {
        IQueryable<Dal.Entities.PurchaseOrder> GetAll();
        Task<Dal.Entities.PurchaseOrder> AddPurchaseOrder(Dal.Entities.PurchaseOrder purchaseOrder);
        Task<Dal.Entities.PurchaseOrder> UpdatePurchaseOrder(Dal.Entities.PurchaseOrder purchaseOrder);
        Task<Dal.Entities.PurchaseOrder> GetPurchaseOrderById(long id);
        Task<Dal.Entities.PurchaseOrder> SetPurchaseOrderStatus(long purchaseOrderId, PurchaseOrderStatus status);
        Task<Dal.Entities.PurchaseOrder> MarkAsPrinted(long id);
        PurchaseOrderStatus GetPurchaseOrderStatus(Dal.Entities.PurchaseOrder purchaseOrder);
    }
}