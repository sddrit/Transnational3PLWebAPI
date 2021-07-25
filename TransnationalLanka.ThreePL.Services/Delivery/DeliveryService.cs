using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Product;

namespace TransnationalLanka.ThreePL.Services.Delivery
{
    public interface IDeliveryService
    {
        IQueryable<Dal.Entities.Delivery> GetDeliveries();
        Task<Dal.Entities.Delivery> CreateDelivery(Dal.Entities.Delivery delivery);
        Task<Dal.Entities.Delivery> GetDeliveryById(long id);
        Task<Dal.Entities.Delivery> MarkAsProcessing(long id, int requiredTrackingNumberCount);
        Task<Dal.Entities.Delivery> MarkAsDispatch(long id, long warehouseId);
        Task<Dal.Entities.Delivery> MarkAsComplete(long id);
        Task<Dal.Entities.Delivery> MarkAsReturn(long id);
        Task<Dal.Entities.Delivery> MarkAsCustomerReturn(long id);
    }

    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockService _stockService;

        public DeliveryService(IUnitOfWork unitOfWork, IStockService stockService)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
        }

        public IQueryable<Dal.Entities.Delivery> GetDeliveries()
        {
            return _unitOfWork.DeliveryRepository.GetAll();
        }

        public async Task<Dal.Entities.Delivery> CreateDelivery(Dal.Entities.Delivery delivery)
        {
            delivery.DeliveryStatus = DeliveryStatus.Pending;
            delivery.Created = DateTimeOffset.UtcNow;
            _unitOfWork.DeliveryRepository.Insert(delivery);
            await _unitOfWork.SaveChanges();
            return delivery;
        }

        public async Task<Dal.Entities.Delivery> GetDeliveryById(long id)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Supplier)
                .ThenInclude(s => s.Address.City)
                .Include(d => d.WareHouse)
                .Include(d => d.DeliveryItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync();

            if (delivery == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Unable to get delivery by id"
                    }
                });
            }

            return delivery;
        }

        public async Task<Dal.Entities.Delivery> MarkAsProcessing(long id, int requiredTrackingNumberCount)
        {
            var delivery = await GetDeliveryById(id);

            if (!CanMarkAsProcessing(delivery))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Unable to mark as processing"
                    }
                });
            }

            //Todo call to tracking api and get the tracking number and store in our entity

            delivery.TrackingNumbers = new[] {"ABC0001", "ABC0002", "ABC003"};

            delivery.DeliveryStatus = DeliveryStatus.Processing;
            await _unitOfWork.SaveChanges();

            return delivery;
        }

        public async Task<Dal.Entities.Delivery> MarkAsDispatch(long id, long warehouseId)
        {
            await using var transaction = await _unitOfWork.GetTransaction();

            try
            {
                var delivery = await GetDeliveryById(id);

                if (!CanMarkAsDispatch(delivery))
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = "Unable to mark as dispatch"
                        }
                    });
                }

                delivery.WareHouseId = warehouseId;

                foreach (var deliveryItem in delivery.DeliveryItems)
                {
                    await _stockService.AdjustStock(warehouseId, deliveryItem.ProductId, deliveryItem.UnitCost,
                        -deliveryItem.Quantity, null, $"Delivery - {delivery.DeliveryNo}");
                }

                delivery.DeliveryStatus = DeliveryStatus.Dispatched;

                await _unitOfWork.SaveChanges();

                await transaction.CommitAsync();
                return delivery;

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Dal.Entities.Delivery> MarkAsComplete(long id)
        {
            var delivery = await GetDeliveryById(id);

            if (!CanMarkAsComplete(delivery))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Unable to mark as complete"
                    }
                });
            }

            delivery.DeliveryStatus = DeliveryStatus.Completed;
            await _unitOfWork.SaveChanges();

            return delivery;
        }

        public async Task<Dal.Entities.Delivery> MarkAsReturn(long id)
        {
            await using var transaction = await _unitOfWork.GetTransaction();

            try
            {
                var delivery = await GetDeliveryById(id);

                if (!CanMarkAsReturn(delivery))
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = "Unable to mark as return"
                        }
                    });
                }

                foreach (var deliveryItem in delivery.DeliveryItems)
                {
                    await _stockService.AdjustStock(delivery.WareHouseId.Value, deliveryItem.ProductId,
                        deliveryItem.UnitCost,
                        deliveryItem.Quantity, null, $"Delivery Return - {delivery.DeliveryNo}");
                }

                delivery.DeliveryStatus = DeliveryStatus.Return;
                await _unitOfWork.SaveChanges();

                return delivery;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Dal.Entities.Delivery> MarkAsCustomerReturn(long id)
        {
            await using var transaction = await _unitOfWork.GetTransaction();

            try
            {
                var delivery = await GetDeliveryById(id);

                if (!CanMarkAsCustomerReturn(delivery))
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = "Unable to mark as customer return"
                        }
                    });
                }

                foreach (var deliveryItem in delivery.DeliveryItems)
                {
                    await _stockService.AdjustStock(delivery.WareHouseId.Value, deliveryItem.ProductId,
                        deliveryItem.UnitCost,
                        deliveryItem.Quantity, null, $"Delivery Customer Return - {delivery.DeliveryNo}");
                }

                delivery.DeliveryStatus = DeliveryStatus.CustomerReturn;
                await _unitOfWork.SaveChanges();

                return delivery;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private bool CanMarkAsProcessing(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Pending;
        }

        private bool CanMarkAsDispatch(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Processing;
        }

        private bool CanMarkAsComplete(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Dispatched;
        }

        private bool CanMarkAsReturn(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Dispatched;
        }

        private bool CanMarkAsCustomerReturn(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Completed;
        }
    }
}
