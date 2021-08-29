using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Environment;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Supplier;

namespace TransnationalLanka.ThreePL.Services.Delivery
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockService _stockService;
        private readonly ISupplierService _supplierService;
        private readonly IEnvironment _environment;
        private readonly TrackerApiService _trackerApiService;

        public DeliveryService(IUnitOfWork unitOfWork, IStockService stockService,
            ISupplierService supplierService, TrackerApiService trackerApiService,
            IEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _trackerApiService = trackerApiService;
            _supplierService = supplierService;
            _environment = environment;
        }

        public IQueryable<Dal.Entities.Delivery> GetDeliveries()
        {
            return _unitOfWork.DeliveryRepository.GetAll();
        }

        public IQueryable<Dal.Entities.Delivery> GetDeliveries(long supplierId)
        {
            return _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.SupplierId == supplierId);
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
                .Include(d => d.DeliveryCustomer.City)
                .Include(d => d.WareHouse)
                .Include(d => d.DeliveryItems)
                .ThenInclude(i => i.Product)
                .Include(d => d.DeliveryHistories)
                .Include(d => d.DeliveryTrackings)
                .ThenInclude(t => t.DeliveryTrackingItems)
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

            var supplier = await _supplierService.GetSupplierById(delivery.SupplierId);

            var trackingNumbers = new List<string>();

            var response = await _trackerApiService.GetSetTrackingNoRange(new GetSetTrackingNumberDetailsRequest()
            {
                CustomerCode = supplier.TrackerCode,
                ConsignorName = supplier.SupplierName,
                TPLWSBatchID = delivery.DeliveryNo,
                Type = delivery.Type == DeliveryType.Cod ? "1" : "2",
                TrackingNoCount = requiredTrackingNumberCount.ToString(),
                ConsigneeName = delivery.DeliveryCustomer.FullName,
                ConsigneeAddress = delivery.DeliveryCustomer.Address,
                ConsigneePhone = string.IsNullOrEmpty(delivery.DeliveryCustomer.Phone) ? "" : delivery.DeliveryCustomer.Phone,
                ConsigneeCity = delivery.DeliveryCustomer.City.CityName,
                InsertedDate = DateTime.Now,
                CODAmount = delivery.Type == DeliveryType.Cod? delivery.SubTotal.ToString("0.00") : "0.00",
            });

            if (response.IsSuccess == "1")
            {
                trackingNumbers.AddRange(response.Result.TrackingNumber.Split(','));
            }
            else
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new()
                    {
                        Message = response.Message
                    }
                });
            }

            delivery.DeliveryTrackings = new List<DeliveryTracking>();

            foreach (var trackingNumber in trackingNumbers)
            {
                delivery.DeliveryTrackings.Add(new DeliveryTracking()
                {
                    Status = TrackingStatus.Pending,
                    TrackingNumber = trackingNumber,
                    DeliveryTrackingItems = delivery.DeliveryItems.Select(item => new DeliveryTrackingItem()
                    {
                        ProductId = item.ProductId,
                        Quantity = 0,
                        UnitCost = item.UnitCost
                    }).ToList()
                });
            }

            delivery.DeliveryStatus = DeliveryStatus.Processing;

            await _unitOfWork.SaveChanges();

            await AddDeliveryNote(delivery.Id, "Mark as processing");

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

                await AddDeliveryNote(delivery.Id, "Mark as dispatched");


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

            await AddDeliveryNote(delivery.Id, "Mark as complete");

            return delivery;
        }

        public async Task<Dal.Entities.Delivery> MarkAsReturn(long id, string note)
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
                    await _stockService.AdjustReturnStock(delivery.WareHouseId.Value, deliveryItem.ProductId,
                        deliveryItem.UnitCost,
                        deliveryItem.Quantity, null, $"Delivery Return - {delivery.DeliveryNo}");
                }

                delivery.DeliveryStatus = DeliveryStatus.Return;
                await _unitOfWork.SaveChanges();

                await AddDeliveryNote(delivery.Id, $"Mark as return - Reason : {note}");

                await transaction.CommitAsync();

                return delivery;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Dal.Entities.Delivery> MarkAsCustomerReturn(long id, string note)
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
                    await _stockService.AdjustReturnStock(delivery.WareHouseId.Value, deliveryItem.ProductId,
                        deliveryItem.UnitCost,
                        deliveryItem.Quantity, null, $"Delivery Customer Return - {delivery.DeliveryNo}");
                }

                delivery.DeliveryStatus = DeliveryStatus.CustomerReturn;
                await _unitOfWork.SaveChanges();

                await AddDeliveryNote(delivery.Id, $"Mark as customer return - Reason : {note}");

                await transaction.CommitAsync();

                return delivery;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<long> GetDeliveryCount(long supplierId, DateTime from, DateTime to)
        {
            return await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.SupplierId == supplierId && d.DeliveryStatus > DeliveryStatus.Completed &&
                            d.DeliveryDate <= to && d.DeliveryDate >= from)
                .LongCountAsync();
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
            return delivery.DeliveryStatus == DeliveryStatus.Completed;
        }

        private bool CanMarkAsCustomerReturn(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Completed;
        }

        private async Task AddDeliveryNote(long deliveryId, string note)
        {
            var delivery = await GetDeliveryById(deliveryId);
            var currentEnvironment = _environment.GetCurrentEnvironment();
            delivery.DeliveryHistories.Add(new DeliveryHistory()
            {
                Note = note,
                UserName = currentEnvironment.UserName
            });
            await _unitOfWork.SaveChanges();

        }
    }
}
