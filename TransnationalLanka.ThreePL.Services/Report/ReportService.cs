using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Enums.Core;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Delivery;
using TransnationalLanka.ThreePL.Services.Grn;
using TransnationalLanka.ThreePL.Services.Invoice;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.PurchaseOrder;
using TransnationalLanka.ThreePL.Services.Report.Core;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.Services.WareHouse;

namespace TransnationalLanka.ThreePL.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupplierService _supplierService;
        private readonly IWareHouseService _wareHouseService;
        private readonly IGrnService _grnService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IDeliveryService _deliveryService;
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;

        public ReportService(IUnitOfWork unitOfWork, ISupplierService supplierService, IWareHouseService wareHouseService, IGrnService grnService, IPurchaseOrderService purchaseOrderService, IDeliveryService deliveryService, IInvoiceService invoiceService,
            IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _supplierService = supplierService;
            _wareHouseService = wareHouseService;
            _grnService = grnService;
            _purchaseOrderService = purchaseOrderService;
            _deliveryService = deliveryService;
            _invoiceService = invoiceService;
            _productService = productService;
        }

        public async Task<InventoryReport> GetInventoryReport(long? wareHouseId, long? supplierId)
        {
            string supplierName = null;
            string supplierCode = null;
            string wareHouseName = null;
            string wareHouseCode = null;
            string wareHouseAddress1 = null;
            string wareHouseAddress2 = null;

            if (supplierId.HasValue)
            {
                var supplier = await _supplierService.GetSupplierById(supplierId.Value);
                supplierName = supplier.SupplierName;
                supplierCode = supplier.Code;
            }

            if (wareHouseId.HasValue)
            {
                var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId.Value);
                wareHouseName = wareHouse.Name;
                wareHouseAddress1 = wareHouse.Address.AddressLine1;
                wareHouseAddress2 = wareHouse.Address.AddressLine2;
                wareHouseCode = wareHouse.Code;
            }

            var query = _unitOfWork.ProductRepository.GetAll()
                .Where(p => p.Active && p.Supplier.Active)
                .Include(p => p.Stocks)
                .AsQueryable();

            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            if (wareHouseId.HasValue)
            {
                query = query.Where(p => p.Stocks.Any(s => s.WareHouseId == wareHouseId.Value));
            }

            return new InventoryReport()
            {
                SupplierName = supplierName,
                WareHouseCode = wareHouseCode,
                WareHouseName = wareHouseName,
                SupplierCode = supplierCode,
                Date = System.DateTime.Now,
                WareHouseAddressLine1 = wareHouseAddress1,
                WareHouseAddressLine2 = wareHouseAddress2,
                InventoryReportItems = await query.Select(p => new InventoryReportItem()
                {
                    Code = p.Code,
                    UnitPrice = p.UnitPrice,
                    Quantity = p.Stocks.Where(s => wareHouseId.HasValue && s.WareHouseId == wareHouseId.Value || !wareHouseId.HasValue)
                        .Sum(s => s.Quantity),
                    Description = p.Description,
                    UnitOfMeasure = p.MassUnit.ToString(),
                    Value = p.StockAdjustments
                        .Where(a => wareHouseId.HasValue && a.WareHouseId == wareHouseId.Value || !wareHouseId.HasValue)
                        .Where(a => a.Type == StockAdjustmentType.In || a.Type == StockAdjustmentType.Out)
                        .Sum(a => a.Type == StockAdjustmentType.In? a.Quantity * a.UnitCost: -(a.Quantity * a.UnitCost))
                }).ToListAsync()
            };
        }

        public async Task<GrnReport> GetGrnReport(long id)
        {
            var grn = await _grnService.GetByIdIncludeWithProduct(id);
            var supplier = await _supplierService.GetSupplierById(grn.SupplierId);
            var wareHouse = await _wareHouseService.GetWareHouseById(grn.WareHouseId);
            Dal.Entities.PurchaseOrder purchaseOrder = !grn.PurchaseOrderId.HasValue ? null : await _purchaseOrderService.GetPurchaseOrderById(grn.PurchaseOrderId.Value);

            return new GrnReport()
            {
                GrnNo = grn.GrnNo,
                Date = grn.Created,
                GrnType = grn.Type.GetDescription(),
                PurchaseOrderNumber = purchaseOrder?.PoNumber,
                SupplierName = supplier.SupplierName,
                SupplierCode = supplier.Code,
                SupplierInvoiceNo=grn?.SupplierInvoiceNumber,
                WareHouse = wareHouse.Code,
                WareHouseName = wareHouse.Name,
                WareHouseAddressLine1 = wareHouse.Address.AddressLine1,
                WareHouseAddressLine2 = wareHouse.Address.AddressLine2,
                City=wareHouse.Address.City.CityName,
                Telephone = wareHouse?.Phone,                
                GrnReportItems = grn.GoodReceivedNoteItems.Select(item => new GrnReportItem()
                {
                    ExpiredDate = item.ExpiredDate,
                    ProductId = item.Product.Code,
                    ProductName = item.Product.Description,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    UnitOfMeasure = item.Product.MassUnit.ToString()
                }).ToList()
            };
        }

        public async Task<List<WayBill>> GetWayBill(long id)
        {
            var delivery = await _deliveryService.GetDeliveryById(id);

            List<WayBill> WayBills = new List<WayBill>();

            if (delivery.WareHouse == null)
            {
                return new List<WayBill>();
            }

            foreach (var item in delivery.DeliveryTrackings)
            {
                WayBill waybill = new WayBill()
                {
                    DeliveryName = delivery.DeliveryCustomer.FullName,
                    DeliveryAddress = delivery.DeliveryCustomer.Address,
                    DeliveryNumber = delivery.DeliveryNo,
                    DeliveryPrice = delivery.Type == DeliveryType.Cod? item.DeliveryTrackingItems.Sum(x => x.Value) : 0,
                    SupplierCode = delivery.Supplier.Code,
                    SupplierName = delivery.Supplier.SupplierName,
                    WareHouseAddress = delivery.WareHouse.Address.ToString(),
                    WareHouseCode = delivery.WareHouse.Code,
                    WareHouseName = delivery.WareHouse.Name,
                    TrackingNo = item.TrackingNumber,
                    WayBillItems = item.DeliveryTrackingItems.Where(i => i.Quantity > 0)
                        .Select(i => new WayBillItem()
                    {
                        ItemCode = i.Product.Code,
                        ItemDescription = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitOfMeasure = i.Product.MassUnit.ToString(),
                        UnitPrice = i.UnitCost,
                        UnitWeight = i.Product.Weight

                    }).ToList()
                };
                WayBills.Add(waybill);

            }
            return WayBills.OrderBy(x => x.TrackingNo).ToList();
        }


        public async Task<PurchaseOrderReport> GetPurchaseOrderReport(long id)
        {
            var po = await _purchaseOrderService.GetPurchaseOrderById(id);
            var supplier = await _supplierService.GetSupplierById(po.SupplierId);

            var wareHouse = po.WareHouseId.HasValue? await _wareHouseService.GetWareHouseById((long)po.WareHouseId): null;

            return new PurchaseOrderReport()
            {

                Date = po.Created,
                PurchaseOrderNumber = po.PoNumber,
                SupplierName = supplier.SupplierName,
                SupplierCode = supplier.Code,
                WareHouse = wareHouse?.Code,
                WareHouseName = wareHouse?.Name,
                WareHouseAddressLine1 = wareHouse?.Address.AddressLine1,
                WareHouseAddressLine2 = wareHouse?.Address.AddressLine2,
                City = wareHouse?.Address.City.CityName,     
                Telephone= wareHouse?.Phone,
                PurchaseOrderReportItems = po.PurchaseOrderItems.Select(item => new PurchaseOrderReportItem()
                {
                    ProductId = item.Product.Code,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                }).ToList()
            };
        }

        public async Task<InventoryMovementReport> GetInventoryMovementReport(long productId, DateTime fromDate, DateTime toDate, long? wareHouseId)
        {
            var from = fromDate;
            var to = toDate.AddDays(1);

            string wareHouseName = null;
            string wareHouseCode = null;
            string wareHouseAddress1 = null;
            string wareHouseAddress2 = null;

            if (wareHouseId.HasValue)
            {
                var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId.Value);
                wareHouseName = wareHouse.Name;
                wareHouseAddress1 = wareHouse.Address.AddressLine1;
                wareHouseAddress2 = wareHouse.Address.AddressLine2;
                wareHouseCode = wareHouse.Code;
            }

            var product = await _productService.GetProductById(productId);

            var query = _unitOfWork.ProductStockAdjustmentRepository.GetAll()
                .Where(p => p.ProductId == productId && p.Created >= from && p.Created <= to)
                .AsQueryable();


            if (wareHouseId.HasValue)
            {
                query = query.Where(p => p.WareHouseId == wareHouseId);
            }

            return new InventoryMovementReport()
            {
                From = fromDate,
                To = toDate,
                WareHouseCode = wareHouseCode,
                WareHouseName = wareHouseName,
                WareHouseAddressLine1 = wareHouseAddress1,
                WareHouseAddressLine2 = wareHouseAddress2,
                ProductCode = product.Code,
                ProductName = product.Name,
                UnitOfMeasure = product.UnitOfMeasure?.ToString(),
                InventoryMovementReportItems = await query.Select(p => new InventoryMovementReportItem()
                {
                    Date = p.Created,
                    UnitPrice = p.UnitCost,
                    Note = p.Note,
                    Quantity = p.Quantity,
                    TypeName = p.Type.GetDescription(),
                    Type = p.Type
                }).ToListAsync()
            };
        }

        public async Task<InvoiceReport> GetInvoice(long id)
        {
            var invoice = await _invoiceService.GetInvoice(id);
            var supplier = await _supplierService.GetSupplierById(invoice.SupplierId);

            return new InvoiceReport()
            {
                From = invoice.From,
                To = invoice.To,
                InvoiceNo = invoice.InvoiceNo,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.Tax,
                NetTotal = invoice.Total,
                TaxPercentage = invoice.TaxPercentage * 100,
                TaxType = invoice.TaxType,
                SupplierCode = supplier.Code,
                SupplierName = supplier.SupplierName,
                SupplierAddressLine1 = supplier.Address.AddressLine1,
                SupplierAddressLine2 = supplier.Address.AddressLine2,
                SupplierCity=supplier.Address.City.CityName,
                VatNumber = supplier.VatNumber,                         
                InvoiceReportItems = invoice.InvoiceItems.Select(item => new InvoiceReportItem()
                {
                    Description = item.Description,
                    Rate = item.Rate,
                    Quantity = item.Quantity,
                    Amount = item.Amount,
                    ChargeType= item.Type
                }).ToList()
            };
        }

        public async Task<SellerWiseItemReport> GetSellerWiseItemDetail(long wareHouseId, long supplierId)
        {
            var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId);
            var supplier = await _supplierService.GetSupplierById(supplierId);

            var query = _unitOfWork.ProductRepository.GetAll()
                .Where(p => p.Active && p.Supplier.Active)
                .Include(p => p.Stocks)
                .AsQueryable();

            query = query.Where(p => p.SupplierId == supplierId);
            query = query.Where(p => p.Stocks.Any(s => s.WareHouseId == wareHouseId));

            return new SellerWiseItemReport()
            {
                WareHouseAddressLine1 = wareHouse.Address.AddressLine1,
                WareHouseName = wareHouse.Name,
                WareHouseAddressLine2 = wareHouse.Address.AddressLine2,
                WareHouseCode = wareHouse.Code,
                SupplierCode = supplier.Code,
                SupplierName = supplier.SupplierName,
                SellerWiseItemReportDetails = query.Select(item => new SellerWiseItemReportDetail()
                {
                    Code = item.Code,
                    Description = item.Description,
                    Quantity = item.Stocks.Where(s => s.WareHouseId == wareHouseId).Sum(s => s.Quantity),
                    UnitOfMeasure = item.UnitOfMeasure.Code,
                    Value = item.StockAdjustments
                        .Where(a => a.WareHouseId == wareHouseId && (a.Type == StockAdjustmentType.In || a.Type == StockAdjustmentType.Out))
                        .Sum(a => a.Type == StockAdjustmentType.In ? a.Quantity * a.UnitCost : -(a.Quantity * a.UnitCost)),
                    UnitPrice = item.UnitPrice,
                }).ToList()
            };
        }

        public async Task<MonthlyReconsilationReport> GetMonthlyReconsilationReport(DateTime fromDate, DateTime toDate, long wareHouseId)
        {
            var from = fromDate;
            var to = toDate.AddDays(1);

            var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId);

            var oldTotalReceivedGrn = await _unitOfWork.GoodReceiveNoteRepository.GetAll()
                .Where(g => g.Created < from && g.Type == GrnType.Received)
                .SelectMany(g => g.GoodReceivedNoteItems)
                .SumAsync(i => i.UnitCost * i.Quantity);

            var oldReturnGrn = await _unitOfWork.GoodReceiveNoteRepository.GetAll()
                .Where(g => g.Created < from && g.Type == GrnType.Return)
                .SelectMany(g => g.GoodReceivedNoteItems)
                .SumAsync(i => i.UnitCost * i.Quantity);

            var oldReturnFromCustomer = await _unitOfWork.ProductStockAdjustmentRepository.GetAll()
                .Where(a => a.Created < from && a.Type == StockAdjustmentType.SalesReturnIn)
                .SumAsync(a => a.UnitCost * a.Quantity);

            var oldDamageGoods = await _unitOfWork.ProductStockAdjustmentRepository.GetAll()
                .Where(a => a.Created < from && a.Type == StockAdjustmentType.DamageIn)
                .SumAsync(a => a.UnitCost * a.Quantity);

            var oldTotalSales = await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.DeliveryDate < from && d.DeliveryStatus != DeliveryStatus.Return)
                .SelectMany(d => d.DeliveryItems)
                .SumAsync(i => i.Quantity * i.UnitCost);

            var openingBalance = oldTotalReceivedGrn - oldReturnGrn - oldTotalSales + oldReturnFromCustomer -
                                 oldDamageGoods;

            var currentTotalReceivedGrn = await _unitOfWork.GoodReceiveNoteRepository.GetAll()
                .Where(g => g.Created >= from && g.Created < to && g.Type == GrnType.Received)
                .SelectMany(g => g.GoodReceivedNoteItems)
                .SumAsync(i => i.UnitCost * i.Quantity);

            var currentTotalReturnGrn = await _unitOfWork.GoodReceiveNoteRepository.GetAll()
                .Where(g => g.Created < from && g.Type == GrnType.Return)
                .SelectMany(g => g.GoodReceivedNoteItems)
                .SumAsync(i => i.UnitCost * i.Quantity);

            var currentTotalSales = await _unitOfWork.DeliveryRepository.GetAll()
                .Where(d => d.DeliveryDate >= from && d.DeliveryDate < to &&
                            d.DeliveryStatus != DeliveryStatus.Return)
                .SelectMany(d => d.DeliveryItems)
                .SumAsync(i => i.Quantity * i.UnitCost);

            var currentDamagedGoods = await _unitOfWork.ProductStockAdjustmentRepository.GetAll()
                .Where(a => a.Created >= from && a.Created < to && a.Type == StockAdjustmentType.DamageIn)
                .SumAsync(a => a.UnitCost * a.Quantity);

            var currentReturnFromCustomer = await _unitOfWork.ProductStockAdjustmentRepository.GetAll()
                .Where(a => a.Created >= from && a.Created < to && a.Type == StockAdjustmentType.SalesReturnIn)
                .SumAsync(a => a.UnitCost * a.Quantity);

            return new MonthlyReconsilationReport()
            {
                WareHouse = wareHouse.Code,
                ReconsilationDate = toDate,
                WareHouseAddressLine1 = wareHouse.Address.AddressLine1,
                WareHouseAddressLine2 = wareHouse.Address.AddressLine2,
                WareHouseName = wareHouse.Name,
                OpeningBalance = openingBalance,
                TotalReceivedGRN = currentTotalReceivedGrn,
                TotalReturnGRN = -currentTotalReturnGrn,
                TotalSales = -currentTotalSales,
                DamagedGoods = -currentDamagedGoods,
                ReturnsFromCustomer = currentReturnFromCustomer
            };
        }

        public async Task<MonthlySalesReport> GetMonthlySalesReport(DateTime fromDate, DateTime toDate, long wareHouseId)
        {
            var from = fromDate;
            var to = toDate.AddDays(1);

            var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId);

            var monthlySalesReportItems = await _unitOfWork.DeliveryItemRepository.GetAll()
                .Include(i => i.Delivery)
                .Where(i =>
                    i.Delivery.WareHouseId.Equals(wareHouseId) &&
                            i.Delivery.DeliveryStatus != DeliveryStatus.Return &&
                            i.Delivery.DeliveryDate >= from &&
                            i.Delivery.DeliveryDate <= to)
                .Select(i => new MonthlySalesReportItem()
                {
                    Code = i.Product.Code,
                    UnitPrice = i.UnitCost,
                    Quantity = i.Quantity,
                    Name = i.Product.Name,
                    UnitOfMeasure = i.Product.UnitOfMeasure.Code,
                    Date = i.Delivery.DeliveryDate,
                    DeliveryNumber = i.Delivery.DeliveryNo
                }).ToListAsync();

            return new MonthlySalesReport()
            {
                WareHouseCode = wareHouse.Code,
                WareHouseAddressLine1 = wareHouse.Address.AddressLine1,
                WareHouseAddressLine2 = wareHouse.Address.AddressLine2,
                WareHouseName = wareHouse.Name,
                FromDate = fromDate,
                ToDate = toDate,
                MonthlySalesReportItems = monthlySalesReportItems
            };

        }

        public async Task<SellerWiseItemSummary> GetSellerWiseItemSummary(long wareHouseId)
        {
            //to check the logic
            var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId);

            var supplierProduct = _productService.GetProducts().GroupBy(x => new { x.Supplier.Code, x.Supplier.SupplierName, x.MassUnit })
                   .Select(sp => new SellerWiseItemSummaryDetail
                   {
                       SupplierName = sp.Key.SupplierName,
                       SupplierCode = sp.Key.Code,
                       Quantity = 2, //how to calculate this?
                       Value = sp.Sum(c => c.UnitPrice)
                   }).ToList();

            return new SellerWiseItemSummary()
            {
                WareHouseCode = wareHouse.Code,
                WareHouseAddressLine1 = wareHouse.Address.AddressLine1,
                WareHouseAddressLine2 = wareHouse.Address.AddressLine2,
                WareHouseName = wareHouse.Name,
                SellerWiseItemReportDetails = supplierProduct.Select(item => new SellerWiseItemSummaryDetail()
                {
                    SupplierCode = item.SupplierCode,
                    SupplierName = item.SupplierName,
                    Quantity = item.Quantity,
                    UOM = item.UOM,
                    Value = item.Value
                }).ToList()
            };
        }

        public async Task<StockAdjustmentReport> GetStockAdjustmentReport(long wareHouseId, long supplierId)
        {
            //to write the logic
            var wareHouse = await _wareHouseService.GetWareHouseById(wareHouseId);

            var product = _productService.GetProducts();

            return new StockAdjustmentReport()
            {
                WareHouseCode = wareHouse.Code,
                WareHouseAddressLine1 = wareHouse.Address.AddressLine1,
                WareHouseAddressLine2 = wareHouse.Address.AddressLine2,
                WareHouseName = wareHouse.Name,
                StockAdjustmentItemDetails = product.Select(item => new StockAdjustmentItemDetail()
                {
                    Code = item.Code,
                    Description = item.Name,
                    Quantity = 2,
                    UnitOfMeasure = item.MassUnit.ToString(),
                    UnitPrice = item.UnitPrice,
                    Remark = "Test"
                }).ToList()

            };

        }

        public async Task<ItemWiseReOrderLevelReport> GetReOrderLevelReport(long? supplierId)
        {
            Dal.Entities.Supplier supplier = null;

            if (supplierId.HasValue)
            {
                supplier = await _supplierService.GetSupplierById(supplierId.Value);
            }

            var query = _unitOfWork.ProductRepository.GetAll();

            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            query = query.Where(p => p.ReorderLevel >= p.Stocks.Sum(s => s.Quantity));

            return new ItemWiseReOrderLevelReport()
            {
                SupplierCode = supplier?.Code,
                SupplierName = supplier?.SupplierName,
                ItemWiseReOrderLevelDetails = query.Select(item => new ItemWiseReOrderLevelDetailReport()
                {
                    Code = item.Code,
                    Name = item.Name,
                    UnitOfMeasure = item.UnitOfMeasure.Code,
                    ReOrderLevel = item.ReorderLevel,
                    StockInHand = item.Stocks.Sum(s => s.Quantity)
                }).ToList()
            };
        }
    }

}


