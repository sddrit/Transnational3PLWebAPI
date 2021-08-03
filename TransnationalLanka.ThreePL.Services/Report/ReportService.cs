using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Delivery;
using TransnationalLanka.ThreePL.Services.Grn;
using TransnationalLanka.ThreePL.Services.Invoice;
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

        public ReportService(IUnitOfWork unitOfWork, ISupplierService supplierService, IWareHouseService wareHouseService, IGrnService grnService, IPurchaseOrderService purchaseOrderService, IDeliveryService deliveryService, IInvoiceService invoiceService)
        {
            _unitOfWork = unitOfWork;
            _supplierService = supplierService;
            _wareHouseService = wareHouseService;
            _grnService = grnService;
            _purchaseOrderService = purchaseOrderService;
            _deliveryService = deliveryService;
            _invoiceService = invoiceService;
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
                    Quantity = p.Stocks.Sum(s => s.Quantity),
                    Description = p.Description,
                    UnitOfMeasure = p.MassUnit.ToString()
                }).ToListAsync()
            };
        }

        public async Task<GrnReport> GetGrnReport(long id)
        {
            var grn = await _grnService.GetByIdIncludeWithProduct(id);           
            var supplier = await _supplierService.GetSupplierById(grn.SupplierId);
            var wareHouse = await _wareHouseService.GetWareHouseById(grn.WareHouseId);
            Dal.Entities.PurchaseOrder purchaseOrder = !grn.PurchaseOrderId.HasValue? null: await _purchaseOrderService.GetPurchaseOrderById(grn.PurchaseOrderId.Value);

            return new GrnReport()
            {
                GrnNo = grn.GrnNo,
                Date = grn.Created,
                PurchaseOrderNumber = purchaseOrder?.PoNumber,
                SupplierName = supplier.SupplierName,
                SupplierCode = supplier.Code,
                WareHouse = wareHouse.Code,
                WareHouseName = wareHouse.Name,
                WareHouseAddressLine1 = wareHouse.Address.AddressLine1,
                WareHouseAddressLine2 = wareHouse.Address.AddressLine2,
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

        public async Task<WayBill> GetWayBill(long id)
        {
            var delivery = await _deliveryService.GetDeliveryById(id);

            return new WayBill()
            {
                DeliveryName = delivery.DeliveryCustomer.FullName,
                DeliveryAddress = delivery.DeliveryCustomer.Address,
                DeliveryNumber = delivery.DeliveryNo,
                DeliveryPrice = delivery.SubTotal,//?
                SupplierCode = delivery.Supplier.Code,
                SupplierName = delivery.Supplier.SupplierName,
                WareHouseAddress = delivery.WareHouse.Address.AddressLine1 + delivery.WareHouse.Address.AddressLine2,
                WareHouseCode = delivery.WareHouse.Code,
                WareHouseName = delivery.WareHouse.Name,
                TrackingNo = delivery.TrackingNumbers,
                WayBillItems = delivery.DeliveryItems.Select(p => new WayBillItem()
                {
                    ItemCode = p.Product.Code,
                    ItemDescription = p.Product.Description,
                    Quantity = p.Quantity,
                    UnitOfMeasure = p.Product.MassUnit.ToString(),
                    UnitWeight = p.Product.Weight,
                    UnitPrice = p.UnitCost
                }).ToList()

            };
        }

        public async Task<InventoryMovementReport> GetInventoryMovementReport(long? wareHouseId, DateTime fromDate, DateTime toDate, int? productId)
        {
            string supplierName = null;
            string supplierCode = null;
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

            var query = _unitOfWork.ProductStockAdjustmentRepository.GetAll()
                .Where(p => p.Created>= fromDate && p.Created<=toDate )              
                .AsQueryable();
           

            if (wareHouseId.HasValue)
            {
                query = query.Where(p => p.WareHouseId==wareHouseId);
            }

            if (productId.HasValue)
            {
                query = query.Where(p => p.ProductId == productId);
            }    
            
            return new InventoryMovementReport()
            {
                SupplierName = supplierName,
                WareHouseCode = wareHouseCode,
                WareHouseName = wareHouseName,
                SupplierCode = supplierCode,               
                WareHouseAddressLine1 = wareHouseAddress1,
                WareHouseAddressLine2 = wareHouseAddress2,
                InventoryMovementReportItems = await query.Select(p => new InventoryMovementReportItem()
                {
                    Code =p.Product.Code,
                    UnitPrice = p.Product.UnitPrice,
                    Quantity = p.Quantity,
                    Description = p.Product.Description,
                    UnitOfMeasure = p.Product.MassUnit.ToString()
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
                SupplierCode = supplier.Code,
                SupplierName = supplier.SupplierName,
                SupplierAddressLine1 = supplier.Address.AddressLine1,
                SupplierAddressLine2 = supplier.Address.AddressLine2,
                InvoiceReportItems = invoice.InvoiceItems.Select(item => new InvoiceReportItem()
                {
                    Description = item.Description,
                    Amount = item.Amount
                }).ToList()
            };
        }

    }
}
