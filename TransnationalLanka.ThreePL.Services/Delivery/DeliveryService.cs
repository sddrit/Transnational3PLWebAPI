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
using TransnationalLanka.ThreePL.Services.Common.Mapper;
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

        public async Task<List<DeliveryStat>> GetTodayDeliveryStat(long? supplierId = null, long[] wareHouses = null)
        {
            var query = _unitOfWork.DeliveryRepository.GetAll();

            if (supplierId.HasValue)
            {
                query = query.Where(d => d.SupplierId == supplierId);
            }

            if (wareHouses != null && wareHouses.Any())
            {
                query = query.Where(d =>
                    d.WareHouseId == null || (d.WareHouseId.HasValue && wareHouses.Contains(d.WareHouseId.Value)));
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

        public async Task<List<DayDeliveryStat>> GetWeeklyDeliveryStat(long? supplierId = null, long[] wareHouses = null)
        {
            var query = _unitOfWork.DeliveryRepository.GetAll();

            if (supplierId.HasValue)
            {
                query = query.Where(d => d.SupplierId == supplierId);
            }

            if (wareHouses != null && wareHouses.Any())
            {
                query = query.Where(d =>
                    d.WareHouseId == null || (d.WareHouseId.HasValue && wareHouses.Contains(d.WareHouseId.Value)));
            }

            DateTime baseDate = DateTime.Today;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);

            query = query.Where(d => d.DeliveryDate >= thisWeekStart && d.DeliveryDate <= thisWeekEnd);

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
                           Count = i.Sum(s => s.Count),
                           Status = i.Key
                       }).ToList()
                }).ToList();


            //Fill the data

            var deliveryStatus = Enum.GetValues<DeliveryStatus>();

            for (DateTime currentDate = thisWeekStart; currentDate <= thisWeekEnd; currentDate = currentDate.AddDays(1))
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

            return result.OrderBy(i => i.Date).ToList();
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
            delivery.DeliveryDate = delivery.DeliveryDate.Date;
            _unitOfWork.DeliveryRepository.Insert(delivery);
            await _unitOfWork.SaveChanges();
            return delivery;
        }

        public async Task<Dal.Entities.Delivery> UpdateDelivery(Dal.Entities.Delivery delivery)
        {
            var currentDelivery = await GetDeliveryById(delivery.Id);

            if (currentDelivery.DeliveryStatus != DeliveryStatus.Pending)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Unable update the delivery"
                    }
                });
            }

            var mapper = ServiceMapper.GetMapper();
            mapper.Map(delivery, currentDelivery);

            currentDelivery.DeliveryDate = delivery.DeliveryDate.Date;
            currentDelivery.Updated = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChanges();

            return currentDelivery;
        }

        public async Task<Dal.Entities.Delivery> GetDeliveryById(long id)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Supplier)
                .ThenInclude(s => s.Address.City)
                .Include(d => d.DeliveryCustomer.City)
                .Include(d => d.WareHouse)
                .ThenInclude(w => w.Address.City)
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

            for (int i = 0; i < requiredTrackingNumberCount; i++)
            {
                var trackingNumberResponse = await _trackerApiService.GetTrackingNoRange(new GetTrackingNoRangeRequest()
                {
                    CustomerCode = supplier.TrackerCode,
                    Type = delivery.Type == DeliveryType.Cod ? "1" : "2"
                });


                if (trackingNumberResponse.IsSuccess != "1")
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = "Unable to get the tracking number"
                        }
                    });
                }

                trackingNumbers.Add(trackingNumberResponse.Result.TrackingNumber);
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

                var supplier = await _supplierService.GetSupplierById(delivery.SupplierId);

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
                    await _stockService.AdjustStock(StockAdjustmentType.Out, warehouseId, deliveryItem.ProductId, deliveryItem.UnitCost,
                        deliveryItem.Quantity, null, $"Delivery - {delivery.DeliveryNo}");
                }

                foreach (var deliveryTracking in delivery.DeliveryTrackings)
                {
                    deliveryTracking.Status = TrackingStatus.Dispatched;

                    var response = await _trackerApiService.UpdateTrackingNoDetails(new UpdateTrackingNoDetailRequest()
                    {
                        TrackingNo = deliveryTracking.TrackingNumber,
                        ConsignorName = supplier.TrackerCode,
                        ConsigneeName = delivery.DeliveryCustomer.FullName,
                        ConsigneeAddress = delivery.DeliveryCustomer.Address,
                        ConsigneePhone = string.IsNullOrEmpty(delivery.DeliveryCustomer.Phone) ? "" : delivery.DeliveryCustomer.Phone,
                        ConsigneeCity = delivery.DeliveryCustomer.City.CityName,
                        InsertedDate = DateTime.Now,
                        TplWsBatchId = delivery.DeliveryNo,
                        CodAmount = delivery.Type == DeliveryType.Cod ?
                            deliveryTracking.DeliveryTrackingItems.Sum(i => i.Quantity * i.UnitCost).ToString("0.00") : "0.00",
                    });

                    if (response.IsSuccess != "1")
                    {
                        throw new ServiceException(new ErrorMessage[]
                        {
                            new()
                            {
                                Message = $"Unable to update the tracking number {deliveryTracking.TrackingNumber}"
                            }
                        });
                    }
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
                if (trackingNumbers.Select(tn => tn.ToLower())
                    .Contains(deliveryTracking.TrackingNumber.ToLower()))
                {
                    
                    if (deliveryTracking.Status != TrackingStatus.Dispatched)
                    {
                        throw new ServiceException(new ErrorMessage[]
                        {
                            new ErrorMessage()
                            {
                                Message =
                                    $"Tracking number ({deliveryTracking.TrackingNumber}) could not able to mark as complete"
                            }
                        });
                    }

                    deliveryTracking.Status = TrackingStatus.Completed;
                }
            }

            SetDeliveryStatus(delivery);

            await _unitOfWork.SaveChanges();

            await AddDeliveryNote(delivery.Id, "Mark as complete");

            return delivery;
        }

        public async Task<Dal.Entities.Delivery> MarkAsReturn(long id, string[] trackingNumbers, string note)
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
                    if (trackingNumbers.Select(tn => tn.ToLower()).Contains(deliveryTracking.TrackingNumber.ToLower()))
                    {
                        if (deliveryTracking.Status == TrackingStatus.Returned)
                        {
                            throw new ServiceException(new ErrorMessage[]
                            {
                                new ErrorMessage()
                                {
                                    Message =
                                        $"Tracking number ({deliveryTracking.TrackingNumber}) could not able to mark as return"
                                }
                            });
                        }

                        foreach (var trackingItems in deliveryTracking.DeliveryTrackingItems)
                        {
                            await _stockService.AdjustStock(StockAdjustmentType.DispatchReturnIn,
                                delivery.WareHouseId.Value, trackingItems.ProductId,
                                trackingItems.UnitCost,
                                trackingItems.Quantity, null,
                                $"Delivery Return - {delivery.DeliveryNo} #Tracking {deliveryTracking.TrackingNumber}");
                            deliveryTracking.Status = TrackingStatus.Returned;
                        }
                    }
                }

                SetDeliveryStatus(delivery);

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

        public async Task<decimal> GetLatestDeliveryUnitPrice(long productId)
        {
            var latestDeliveryItem = await _unitOfWork.DeliveryItemRepository.GetAll()
                .Where(di => di.ProductId == productId)
                .OrderByDescending(di => di.Created)
                .FirstOrDefaultAsync();

            if (latestDeliveryItem == null)
            {
                var product = await _productService.GetProductById(productId);
                return product.UnitPrice;
            }

            return latestDeliveryItem.UnitCost;
        }

        public async Task<List<ProcessDeliverCompleteResult>> ProcessDeliverySheet(Stream excelFile)
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

                var trackingNumberCell = row.GetCell(0);
                var trackingNumber = trackingNumberCell.ToString();

                var typeCell = row.GetCell(1);
                var type = typeCell.StringCellValue.Trim().ToLower();

                if (string.IsNullOrEmpty(trackingNumber))
                {
                    continue;
                }

                try
                {
                    var deliveryId = await GetDeliveryByTrackingNumber(trackingNumber.Trim());

                    if (type == "complete")
                    {
                        await MarkAsComplete(deliveryId, new[] { trackingNumber.Trim() });
                    }
                    else if(type == "return")
                    {
                        await MarkAsReturn(deliveryId, new []{ trackingNumber }, "Return marked by processing delivery sheet");
                    }
                    else
                    {
                        result.Add(new ProcessDeliverCompleteResult()
                        {
                            TrackingNumber = trackingNumber,
                            Success = false,
                            Message = $"{type} type is invalid for processing tracking number {trackingNumber}"
                        });
                        continue;
                    }

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

        private void SetDeliveryStatus(Dal.Entities.Delivery delivery)
        {

            if (delivery.DeliveryTrackings.All(t => t.Status == TrackingStatus.Returned))
            {
                delivery.DeliveryStatus = DeliveryStatus.Return;
                return;
            }

            if (delivery.DeliveryTrackings.Any(t => t.Status == TrackingStatus.Returned))
            {
                delivery.DeliveryStatus = DeliveryStatus.PartiallyReturn;
                return;
            }

            if (delivery.DeliveryTrackings.All(t => t.Status == TrackingStatus.Completed))
            {
                delivery.DeliveryStatus = DeliveryStatus.Completed;
                return;
            }

            if (delivery.DeliveryTrackings.Any(t => t.Status == TrackingStatus.Completed))
            {
                delivery.DeliveryStatus = DeliveryStatus.PartiallyCompleted;
            }
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
                   || delivery.DeliveryStatus == DeliveryStatus.PartiallyCompleted 
                   || delivery.DeliveryTrackings.Any(dt => dt.Status == TrackingStatus.Dispatched);
        }

        private bool CanMarkAsReturn(Dal.Entities.Delivery delivery)
        {
            return delivery.DeliveryStatus == DeliveryStatus.Dispatched 
                   || delivery.DeliveryStatus == DeliveryStatus.Completed
                   || delivery.DeliveryStatus == DeliveryStatus.PartiallyCompleted
                   || delivery.DeliveryStatus == DeliveryStatus.PartiallyReturn;
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

        public async Task<List<Dal.Entities.Delivery>> GetDeliveryByDateRange(DateTime fromDate, DateTime toDate)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.DeliveryDate >= fromDate && d.DeliveryDate <= toDate)
                .Include(d => d.Supplier)
                .ThenInclude(s => s.Address.City)
                .Include(d => d.DeliveryCustomer.City)
                .Include(d => d.WareHouse)
                .Include(d => d.DeliveryItems)
                .ThenInclude(i => i.Product)
                .Include(d => d.DeliveryHistories)
                .Include(d => d.DeliveryTrackings)
                .ThenInclude(t => t.DeliveryTrackingItems).ToListAsync();
                

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

        public async Task<GetDeliveryStatusResponse> GetTrackingDetails(string trackingNumber)
        { 
            return await _trackerApiService.GetDeliveryStatus(new GetDeliveryStatusRequest()
            {
                TrackingNumber = trackingNumber
            });
        }

    }
}
