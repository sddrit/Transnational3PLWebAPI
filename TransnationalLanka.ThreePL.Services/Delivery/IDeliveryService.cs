using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;
using TransnationalLanka.ThreePL.Services.Delivery.Core;

namespace TransnationalLanka.ThreePL.Services.Delivery
{
    public interface IDeliveryService
    {
        IQueryable<Dal.Entities.Delivery> GetDeliveries();
        IQueryable<Dal.Entities.Delivery> GetDeliveries(long supplierId);
        Task<Dal.Entities.Delivery> CreateDelivery(Dal.Entities.Delivery delivery);
        Task<Dal.Entities.Delivery> UpdateDelivery(Dal.Entities.Delivery delivery);
        Task<Dal.Entities.Delivery> GetDeliveryById(long id);
        Task<Dal.Entities.Delivery> MarkAsProcessing(long id, int requiredTrackingNumberCount);
        Task<Dal.Entities.Delivery> MarkAsDispatch(long id, long warehouseId);
        Task<Dal.Entities.Delivery> MarkAsComplete(long id, string[] trackingNumbers);
        Task<Dal.Entities.Delivery> MarkAsReturn(long id, string[] trackingNumbers, string note);
        Task<long> GetDeliveryCount(long supplierId, DateTime from, DateTime to);
        Task<Dal.Entities.Delivery> MapDeliveryProduct(Dal.Entities.Delivery delivery);
        Task<List<ProcessDeliverCompleteResult>> ProcessDeliverySheet(Stream excelFile);
        Task<List<Dal.Entities.Delivery>> GetDeliveryByDateRange(DateTime fromDate, DateTime toDate);
        Task<List<DayDeliveryStat>> GetWeeklyDeliveryStat(long? supplierId = null, long[] wareHouses = null);
        Task<List<DeliveryStat>> GetTodayDeliveryStat(long? supplierId = null, long[] wareHouses = null);
        Task<decimal> GetLatestDeliveryUnitPrice(long productId);
        Task<GetInwardResponse> GetTrackingDetails(string trackingNumber);
    }
}