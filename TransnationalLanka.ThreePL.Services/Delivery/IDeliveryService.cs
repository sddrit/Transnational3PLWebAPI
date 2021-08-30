using System;
using System.Linq;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Delivery
{
    public interface IDeliveryService
    {
        IQueryable<Dal.Entities.Delivery> GetDeliveries();
        IQueryable<Dal.Entities.Delivery> GetDeliveries(long supplierId);
        Task<Dal.Entities.Delivery> CreateDelivery(Dal.Entities.Delivery delivery);
        Task<Dal.Entities.Delivery> GetDeliveryById(long id);
        Task<Dal.Entities.Delivery> MarkAsProcessing(long id, int requiredTrackingNumberCount);
        Task<Dal.Entities.Delivery> MarkAsDispatch(long id, long warehouseId);
        Task<Dal.Entities.Delivery> MarkAsComplete(long id);
        Task<Dal.Entities.Delivery> MarkAsReturn(long id, string note);
        Task<Dal.Entities.Delivery> MarkAsCustomerReturn(long id, string note);
        Task<long> GetDeliveryCount(long supplierId, DateTime from, DateTime to);
        Task<Dal.Entities.Delivery> MapDeliveryProduct(Dal.Entities.Delivery delivery);
    }
}