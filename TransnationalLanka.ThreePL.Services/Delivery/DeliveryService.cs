using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Environment;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;
using TransnationalLanka.ThreePL.Services.Delivery.Core;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Supplier;

namespace TransnationalLanka.ThreePL.Services.Delivery
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IEnvironment _environment;
        private readonly TrackerApiService _trackerApiService;

        public DeliveryService(IUnitOfWork unitOfWork, IStockService stockService,
            ISupplierService supplierService, TrackerApiService trackerApiService,
            IProductService productService,
            IEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _trackerApiService = trackerApiService;
            _supplierService = supplierService;
            _productService = productService;
            _environment = environment;
        }

        public async Task<List<DeliveryStat>> GetTodayDeliveryStat(long? supplierId = null)
        {
            var query = _unitOfWork.DeliveryRepository.GetAll();

            if (supplierId.HasValue)
            {
                query = query.Where(d => d.SupplierId == supplierId);
            }

            DateTime now = DateTime.Now.Date;

            query = query.Where(d => d.DeliveryDate == now);

            var result =  await query.GroupBy(d => d.DeliveryStatus)
                .Select(g => new DeliveryStat()
                {
                    Count = g.LongCount(),
                    Status = g.Key
                }).ToListAsync();

            //Fill the stat

            var deliveryStatus = Enum.GetValues<DeliveryStatus>();

            foreach (var deliveryStatusItem in deliveryStatus)
            {
                if (result.All(i => i.Status != deliveryStatusItem))
                {
                    result.Add(new DeliveryStat()
                    {
                        Status = deliveryStatusItem,
                        Count = 0
                    });
                } 
            }

            return result;
        }

        public async Task<List<DayDeliveryStat>> GetMonthlyDeliveryStat(long? supplierId = null)
        {
            var query = _unitOfWork.DeliveryRepository.GetAll();

            if (supplierId.HasValue)
            {
                query = query.Where(d => d.SupplierId == supplierId);
            }

            DateTime now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, 1);
            var to = from.AddMonths(1).AddDays(-1);

            query = query.Where(d => d.DeliveryDate >= from && d.DeliveryDate <= to);

            var groupedDeliveryStatDaily = await query.GroupBy(d => new {d.DeliveryDate, d.DeliveryStatus})
                .Select(g => new {g.Key.DeliveryStatus, g.Key.DeliveryDate, Count = g.Count()})
                .ToListAsync();

            var result = groupedDeliveryStatDaily.GroupBy(g => g.DeliveryDate)
                .Select(g => new DayDeliveryStat()
                {
                   Date = g.Key,
                   DeliveryStats = g.GroupBy(i => i.DeliveryStatus)
                       .Select(i => new DeliveryStat()
                       {
                           Count = i.Count(),
                           Status = i.Key
                       }).ToList()
                }).ToList();


            //Fill the data

            var deliveryStatus = Enum.GetValues<DeliveryStatus>();

            for (DateTime currentDate = from; currentDate <= to; currentDate = currentDate.AddDays(1))
            {
                if (result.All(i => i.Date != currentDate))
                {
                    result.Add(new DayDeliveryStat()
                    {
                        Date = currentDate,
                        DeliveryStats = deliveryStatus.Select(status => new DeliveryStat()
                        {
                            Count = 0,
                            Status = status
                        }).ToList()
                    });

                    continue;
                }

                var resultItem = result.First(i => i.Date == currentDate);

                foreach (var deliveryStatusItem in deliveryStatus)
                {
                    if (resultItem.DeliveryStats.All(i => i.Status != deliveryStatusItem))
                    {
                        resultItem.DeliveryStats.Add(new DeliveryStat()
                        {
                            Status = deliveryStatusItem,
                            Count = 0
                        });
                    }
                }
            }

            return result;
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

        public async Task<Dal.Entities.Delivery> MapDeliveryProduct(Dal.Entities.Delivery delivery)
        {
            var currentDelivery = await GetDeliveryById(delivery.Id);

            var updatedDeliveryTrackingItems =
                delivery.DeliveryTrackings.SelectMany(t => t.DeliveryTrackingItems).ToList();

            foreach (var currentDeliveryDeliveryItem in currentDelivery.DeliveryItems)
            {
                if (currentDeliveryDeliveryItem.Quantity !=
                    updatedDeliveryTrackingItems.Where(i => i.ProductId == currentDeliveryDeliveryItem.ProductId)
                        .Sum(i => i.Quantity))
                {
                    var product = await _productService.GetProductById(currentDeliveryDeliveryItem.ProductId);

                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = $"{product.Name} quantity is invalid for mapping products to tracking numbers"
                        }
                    });
                }
            }

            currentDelivery.DeliveryTrackings.SelectMany(t => t.DeliveryTrackingItems).ToList()
                .ForEach(deliveryTrackingItem =>
                {
                    var updatedDeliveryTrackingItem =
                        updatedDeliveryTrackingItems.FirstOrDefault(i => i.Id == deliveryTrackingItem.Id);

                    if (updatedDeliveryTrackingItem != null)
                    {
                        deliveryTrackingItem.Quantity = updatedDeliveryTrackingItem.Quantity;
                    }
                });

            await _unitOfWork.SaveChanges();

            return currentDelivery;
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

                foreach (var deliveryItem in delivery.DeliveryItems)
                {
                    if (deliveryItem.Quantity != delivery.DeliveryTrackings.SelectMany(i => i.DeliveryTrackingItems)
                        .Where(i => i.ProductId == deliveryItem.ProductId).Sum(i => i.Quantity))
                    {
                        var product = await _productService.GetProductById(deliveryItem.ProductId);

                        throw new ServiceException(new ErrorMessage[]
                        {
                            new()
                            {
                                Message = $"{product.Name} quantity is invalid for mapping products to tracking numbers"
                            }
                        });
                    }
                }

                delivery.WareHouseId = warehouseId;

                foreach (var deliveryItem in delivery.DeliveryItems)
                {
                    await _stockService.AdjustStock(warehouseId, deliveryItem.ProductId, deliveryItem.UnitCost,
                        -deliveryItem.Quantity, null, $"Delivery - {delivery.DeliveryNo}");
                }

                foreach (var deliveryTracking in delivery.DeliveryTrackings)
                {
                    deliveryTracking.Status = TrackingStatus.Dispatched;
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

        public async Task<Dal.Entities.Delivery> MarkAsComplete(long id, string[] trackingNumbers)
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

            foreach (var deliveryTracking in delivery.DeliveryTrackings)
            {
                if (trackingNumbers.Contains(deliveryTracking.TrackingNumber))
                {
                    deliveryTracking.Status = TrackingStatus.Completed;
                }
            }

            delivery.DeliveryStatus = 
                delivery.DeliveryTrackings.All(t => t.Status == TrackingStatus.Completed) ? DeliveryStatus.Completed 
                    : DeliveryStatus.PartiallyCompleted;

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

                foreach (var deliveryTracking in delivery.DeliveryTrackings)
                {
                    foreach (var trackingItems in deliveryTracking.DeliveryTrackingItems)
                    {
                        await _stockService.AdjustReturnStock(delivery.WareHouseId.Value, trackingItems.ProductId,
                            trackingItems.UnitCost,
                            trackingItems.Quantity, null, $"Delivery Return - {delivery.DeliveryNo} #Tracking {deliveryTracking.TrackingNumber}");
                    }

                    deliveryTracking.Status = TrackingStatus.Returned;
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

        public async Task<long> GetDeliveryCount(long supplierId, DateTime from, DateTime to)
        {
            return await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.SupplierId == supplierId && d.DeliveryStatus > DeliveryStatus.Completed &&
                            d.DeliveryDate <= to && d.DeliveryDate >= from)
                .LongCountAsync();
        }

        public async Task<List<ProcessDeliverCompleteResult>> ProcessDeliveryComplete(Stream excelFile)
        {
            var result = new List<ProcessDeliverCompleteResult>();

            excelFile.Position = 0;
            var workbook = new XSSFWorkbook(excelFile);
            var sheet = workbook.GetSheetAt(0);

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                var cell = row.GetCell(0);
                var trackingNumber = cell.StringCellValue;

                try
                {
                    var deliveryId = await GetDeliveryByTrackingNumber(trackingNumber.Trim());

                    await MarkAsComplete(deliveryId, new[] { trackingNumber.Trim() });

                    result.Add(new ProcessDeliverCompleteResult()
                    {
                        TrackingNumber = trackingNumber,
                        Success = true
                    });
                }
                catch (ServiceException e)
                {
                    result.Add(new ProcessDeliverCompleteResult()
                    {
                        TrackingNumber = trackingNumber,
                        Success = false,
                        Message = e.Messages[0].Message
                    });
                }
                catch (Exception e)
                {
                    result.Add(new ProcessDeliverCompleteResult()
                    {
                        TrackingNumber = trackingNumber,
                        Success = false,
                        Message = e.Message
                    });
                }
            }

            return result;
        }

        private async Task<long> GetDeliveryByTrackingNumber(string trackingNumber)
        {
            var deliveryId = await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.DeliveryTrackings
                    .Any(t => t.TrackingNumber.ToLower() == trackingNumber.ToLower()))
                .Select(d => d.Id)
                .FirstOrDefaultAsync();

            if (deliveryId == 0)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = $"Unable to find delivery from tracking number ${trackingNumber}"
                    }
                });
            }

            return deliveryId;
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
            return delivery.DeliveryStatus == DeliveryStatus.Dispatched 
                   || delivery.DeliveryStatus == DeliveryStatus.PartiallyCompleted;
        }

        private bool CanMarkAsReturn(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Dispatched 
                   || delivery.DeliveryStatus == DeliveryStatus.Completed;
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
