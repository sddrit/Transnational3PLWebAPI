using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Delivery;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Setting;
using TransnationalLanka.ThreePL.Services.Supplier;

namespace TransnationalLanka.ThreePL.Services.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupplierService _supplierService;
        private readonly ISettingService _settingService;
        private readonly IStockService _stockService;
        private readonly IDeliveryService _deliveryService;

        public InvoiceService(IUnitOfWork unitOfWork, ISupplierService supplierService, 
            ISettingService settingService, IStockService stockService, IDeliveryService deliveryService)
        {
            _unitOfWork = unitOfWork;
            _supplierService = supplierService;
            _settingService = settingService;
            _stockService = stockService;
            _deliveryService = deliveryService;
        }

        public IQueryable<Dal.Entities.Invoice> GetInvoices()
        {
            DateTime now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, 1);
            var to = from.AddMonths(1).AddDays(-1);
            return _unitOfWork.InvoiceRepository.GetAll().Where(i => i.To < to);
        }

        public async Task<Dal.Entities.Invoice> MarkAsPaid(long id)
        {
            var invoice = await GetInvoice(id);
            invoice.Paid = true;
            await _unitOfWork.SaveChanges();
            return invoice;
        }

        public async Task GenerateInvoices()
        {
            var supplierIds = await _unitOfWork.SupplierRepository.GetAll()
                .Select(s => s.Id)
                .ToListAsync();

            foreach (var supplierId in supplierIds)
            {
                try
                {
                    await GenerateInvoice(supplierId);
                }
                catch
                {
                    //Todo add logger
                }
            }
        }

        public async Task<Dal.Entities.Invoice> GetInvoice(long id)
        {
            var invoice = await _unitOfWork.InvoiceRepository.GetAll()
                .Where(i => i.Id == id)
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync();
            return invoice;
        }

        public async Task<Dal.Entities.Invoice> CreateOrUpdateManualCharges(long id, List<InvoiceItem> invoiceItems)
        {
            var invoice = await GetInvoice(id);

            //Deleted items
            var deletedItems = invoice.InvoiceItems.Where(i =>
                i.Type == InvoiceItemChargeType.ManualCharges && invoiceItems.All(ui => ui.Id != i.Id)).ToList();

            foreach (var deletedItem in deletedItems)
            {
                invoice.InvoiceItems.Remove(deletedItem);
            }

            //Updated items
            var currentManualCharges =
                invoice.InvoiceItems.Where(i => i.Type == InvoiceItemChargeType.ManualCharges).ToList();

            foreach (var manualCharge in currentManualCharges)
            {
                var updatedItem = invoiceItems.FirstOrDefault(i => i.Id == manualCharge.Id);

                if (updatedItem == null)
                {
                    continue;
                }

                manualCharge.Description = updatedItem.Description;
                manualCharge.Quantity = updatedItem.Quantity;
                manualCharge.Rate = updatedItem.Rate;
            }

            //Added items
            var addedItems = invoiceItems.Where(i => i.Id == 0).ToList();

            foreach (var invoiceItem in addedItems)
            {
                invoice.InvoiceItems.Add(invoiceItem);
            }

            await _unitOfWork.SaveChanges();
            return invoice;
        } 

        private async Task GenerateInvoice(long supplierId)
        {
            var supplier = await _supplierService.GetSupplierById(supplierId);

            if (!_supplierService.IsActiveSupplier(supplier))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new()
                    {
                        Message = $"{supplier.SupplierName} is not a active one"
                    }
                });
            }

            DateTime now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, 1);
            var to = from.AddMonths(1).AddDays(-1);

            var currentInvoice = await GetInvoice(supplierId, from, to);

            if (currentInvoice == null)
            {
                await CreateNewInvoice(supplierId, from, to);
            }
            else
            {
                await UpdateInvoice(supplierId, from, to);
            }
        }

        private async Task<Dal.Entities.Invoice> CreateNewInvoice(long supplierId, DateTime from, DateTime to)
        {
            var taxPercentage = await _settingService.GetValue(Settings.TAX_PERCENTAGE);

            var supplier = await _supplierService.GetSupplierById(supplierId);

            var invoice = new Dal.Entities.Invoice()
            {
                SupplierId = supplierId,
                From = from,
                To = to,
                TaxType = supplier.TaxType,
                InvoiceItems = new List<InvoiceItem>(),
                TaxPercentage = decimal.Parse(taxPercentage)
            };

            invoice.InvoiceItems.Add(new InvoiceItem()
            {
                Rate = supplier.SupplierCharges.StorageChargePerUnit,
                Quantity = supplier.SupplierCharges.AllocatedUnits,
                Type = InvoiceItemChargeType.StorageCharge,
                Description = "Warehouse Rent Charge"
            });

            var storageUnitCount = await _stockService.GetTotalStorage(supplierId);

            if (storageUnitCount > supplier.SupplierCharges.AllocatedUnits)
            {
                var days = DateTime.DaysInMonth(from.Year, from.Month);

                invoice.InvoiceItems.Add(new InvoiceItem()
                {
                    Date = DateTime.Now.Date,
                    Quantity = storageUnitCount - supplier.SupplierCharges.AllocatedUnits,
                    Rate = supplier.SupplierCharges.AdditionalChargePerUnitPrice / days,
                    Type = InvoiceItemChargeType.StorageAdditionalCharge,
                    Description = "Additional Warehouse Rent Charge"
                });
            }

            var deliveryCount = await _deliveryService.GetDeliveryCount(supplierId, from, to);

            invoice.InvoiceItems.Add(new InvoiceItem()
            {
                Date = DateTime.Now.Date,
                Quantity = deliveryCount,
                Rate = supplier.SupplierCharges.HandlingCharge,
                Type = InvoiceItemChargeType.PackageCharge,
                Description = "Processing Charge (Including packing)"
            });

            _unitOfWork.InvoiceRepository.Insert(invoice);
            await _unitOfWork.SaveChanges();

            return invoice;
        }

        private async Task<Dal.Entities.Invoice> UpdateInvoice(long supplierId, DateTime from, DateTime to)
        {
            var taxPercentage = await _settingService.GetValue(Settings.TAX_PERCENTAGE);

            var invoice = await GetInvoice(supplierId, from, to);
            var supplier = await _supplierService.GetSupplierById(supplierId);

            invoice.TaxType = supplier.TaxType;
            invoice.TaxPercentage = decimal.Parse(taxPercentage);

            var storageAmountItem = invoice.InvoiceItems.First(i => i.Type == InvoiceItemChargeType.StorageCharge);
            storageAmountItem.Rate = supplier.SupplierCharges.StorageChargePerUnit;
            storageAmountItem.Quantity = supplier.SupplierCharges.AllocatedUnits;

            var storageUnitCount = await _stockService.GetTotalStorage(supplierId);

            if (storageUnitCount > supplier.SupplierCharges.AllocatedUnits)
            {
                var days = DateTime.DaysInMonth(from.Year, from.Month);

                var additionalStorageAmountItem =
                    invoice.InvoiceItems.FirstOrDefault(i => i.Type == InvoiceItemChargeType.StorageAdditionalCharge);

                if (additionalStorageAmountItem == null)
                {
                    invoice.InvoiceItems.Add(new InvoiceItem()
                    {
                        Date = DateTime.Now.Date,
                        Rate = supplier.SupplierCharges.AdditionalChargePerUnitPrice / days,
                        Quantity = storageUnitCount - supplier.SupplierCharges.AllocatedUnits,
                        Type = InvoiceItemChargeType.StorageAdditionalCharge,
                        Description = "Additional Warehouse Rent Charge"
                    });
                }
                else if(additionalStorageAmountItem.Date < DateTime.Now.Date)
                {
                    additionalStorageAmountItem.Date = DateTime.Now.Date;
                    additionalStorageAmountItem.Quantity += storageUnitCount - supplier.SupplierCharges.AllocatedUnits;
                    additionalStorageAmountItem.Rate = supplier.SupplierCharges.AdditionalChargePerUnitPrice / days;
                }
            }

            var deliveryCount = await _deliveryService.GetDeliveryCount(supplierId, from, to);

            var deliveryAmountItem = invoice.InvoiceItems.First(i => i.Type == InvoiceItemChargeType.PackageCharge);
            deliveryAmountItem.Quantity = deliveryCount;
            deliveryAmountItem.Rate = supplier.SupplierCharges.HandlingCharge;

            await _unitOfWork.SaveChanges();

            return invoice;
        }

        private async Task<Dal.Entities.Invoice> GetInvoice(long supplierId, DateTime from, DateTime to)
        {
            var invoice = await _unitOfWork.InvoiceRepository.GetAll()
                .Where(i => i.SupplierId == supplierId && i.From == from && i.To == to)
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync();
            return invoice;
        }
    }
}
