using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Product.Core;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.Services.WareHouse;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public class StockService : IStockService
    {
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IWareHouseService _wareHouseService;

        private readonly IUnitOfWork _unitOfWork;

        public StockService(ISupplierService supplierService, 
            IProductService productService, IWareHouseService wareHouseService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _supplierService = supplierService;
            _productService = productService;
            _wareHouseService = wareHouseService;
        }

        public IQueryable<ProductStockAdjustment> GetStockAdjustmentsByProductId(long id)
        {
            return _unitOfWork.ProductStockAdjustmentRepository.GetAll()
                 .Where(g => g.ProductId == id);
        }

        public IQueryable<ProductStock> GetStocksByProductId(long id)
        {
            return _unitOfWork.ProductStockRepository.GetAll()
                 .Where(g => g.ProductId == id);
        }

        public async Task<decimal> GetRemainStorage(long supplierId)
        {
            var supplier = await _supplierService.GetSupplierById(supplierId);
            var totalStorage = await GetTotalStorage(supplierId);
            return supplier.SupplierCharges.AllocatedUnits - totalStorage;
        }

        public async Task<decimal> GetTotalStorage(long supplierId)
        {
            return await _unitOfWork.ProductStockRepository.GetAll()
                .Include(s => s.Product)
                .Where(s => s.Product.SupplierId == supplierId)
                .SumAsync(s => (s.Quantity + s.DamageStockQuantity + s.DispatchReturnQuantity + s.SalesReturnQuantity) 
                               * (s.Product.Width * s.Product.Height * s.Product.Length));
        }

        public async Task<List<TotalStorageByWareHouse>> GetTotalStorageByWareHouses(long supplierId)
        {
            return await _unitOfWork.ProductStockRepository.GetAll()
                .Include(s => s.WareHouse)
                .Include(s => s.Product)
                .Where(s => s.Product.SupplierId == supplierId)
                .Select(s => new { s.WareHouseId, s.WareHouse.Name, s.WareHouse.Code, 
                    s.Product.Width, s.Product.Height, s.Product.Length, s.Quantity, s.DamageStockQuantity, 
                    s.DispatchReturnQuantity, s.SalesReturnQuantity })
                .GroupBy(s => new {s.WareHouseId, s.Code, s.Name})
                .Select(g => new TotalStorageByWareHouse()
                {
                    WareHouseId = g.Key.WareHouseId,
                    WareHouseCode = g.Key.Code,
                    WareHouseName = g.Key.Name,
                    TotalStorage = g.Sum(i => (i.Quantity + i.DamageStockQuantity + 
                                                i.DispatchReturnQuantity + i.SalesReturnQuantity) * (i.Width * i.Height * i.Length))
                }).ToListAsync();
        }

        public async Task<decimal> CalculateStorage(CalculateStorageByProducts calculateStorageByProducts)
        {
            var totalStorage = 0m;

            foreach(var product in calculateStorageByProducts.Products)
            {
                totalStorage += await _unitOfWork.ProductRepository.GetAll()
                    .Where(p => p.Id == product.ProductId)
                    .Select(p => ((p.Width * p.Height * p.Length) * product.Quantity))
                    .FirstOrDefaultAsync();
            }

            return totalStorage;
        }

        public async Task TransferDispatchReturnStock(long warehouseId, long productId, decimal unitCost, decimal quantity, decimal damageQuantity, DateTime? expiredDate,
            string note, string trackingNumber)
        {
            var fullNote = $"Transfer dispatch return stock {trackingNumber}, Note - {note}";
            await AdjustStock(StockAdjustmentType.DispatchReturnOut ,warehouseId, productId, unitCost, quantity + damageQuantity, expiredDate, fullNote);
            await AdjustStock(StockAdjustmentType.In, warehouseId, productId, unitCost, quantity, expiredDate,
                    fullNote);
            await AdjustStock(StockAdjustmentType.DamageIn, warehouseId, productId, unitCost, damageQuantity, expiredDate, fullNote);
        }

        public async Task TransferSalesReturnStock(long warehouseId, long productId, decimal unitCost, decimal quantity, decimal damageQuantity, DateTime? expiredDate,
            string note, string trackingNumber)
        {
            if (quantity < 0)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Quantity should be greater than or equal to zero"
                    }
                });
            }

            if (damageQuantity < 0)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Damage quantity should be greater than or equal to zero"
                    }
                });
            }

            var fullNote = $"Transfer sales return stock {trackingNumber}, Note - {note}";
            await AdjustStock(StockAdjustmentType.SalesReturnOut, warehouseId, productId, unitCost, quantity + damageQuantity, expiredDate, fullNote);
            await AdjustStock(StockAdjustmentType.In, warehouseId, productId, unitCost, quantity, expiredDate, fullNote);
            await AdjustStock(StockAdjustmentType.DamageIn, warehouseId, productId, unitCost, damageQuantity, expiredDate, fullNote);
        }

        public async Task AdjustStock(StockAdjustmentType stockAdjustmentType, long warehouseId, long productId, decimal unitCost, decimal quantity, DateTime? expiredDate, 
            string note)
        {

            if (quantity <= 0)
            {
                return;
            }

            var wareHouse = await _wareHouseService.GetWareHouseById(warehouseId);

            if (!_wareHouseService.IsActiveWareHouse(wareHouse))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new()
                    {
                        Message = "Warehouse is not active"
                    }
                });
            }

            var product = await _productService.GetProductById(productId);

            if (!_productService.IsActiveProduct(product))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new()
                    {
                        Message = "Product is not active"
                    }
                });
            }
            
            //Check if already exists product stock

            var productStock = await _unitOfWork.ProductStockRepository.GetAll()
                .FirstOrDefaultAsync(s => s.WareHouseId == warehouseId && s.ProductId == productId);

            if (productStock == null)
            {
                productStock = new ProductStock()
                {
                    ProductId = productId,
                    Quantity = 0,
                    WareHouseId = warehouseId
                };

                _unitOfWork.ProductStockRepository.Insert(productStock);
            }

            //Grn Stock Adjustments

            if (stockAdjustmentType == StockAdjustmentType.In)
            {
                productStock.Quantity += quantity;
            }

            if (stockAdjustmentType == StockAdjustmentType.Out)
            {
                if (productStock.Quantity - quantity < 0)
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new()
                        {
                            Message = "Insufficient Quantity"
                        }
                    });
                }

                productStock.Quantity -= quantity;
            }

            //Damage Adjustments

            if (stockAdjustmentType == StockAdjustmentType.DamageIn)
            {
                productStock.DamageStockQuantity += quantity;
            }

            if (stockAdjustmentType == StockAdjustmentType.DamageOut)
            {
                if (productStock.DamageStockQuantity - quantity < 0)
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new()
                        {
                            Message = "Insufficient Quantity"
                        }
                    });
                }

                productStock.DamageStockQuantity -= quantity;
            }

            //Dispatch Return Adjustments

            if (stockAdjustmentType == StockAdjustmentType.DispatchReturnIn)
            {
                productStock.DispatchReturnQuantity += quantity;
            }

            if (stockAdjustmentType == StockAdjustmentType.DispatchReturnOut)
            {
                if (productStock.DispatchReturnQuantity - quantity < 0)
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new()
                        {
                            Message = "Insufficient Quantity"
                        }
                    });
                }

                productStock.DispatchReturnQuantity -= quantity;
            }

            //Sales Return Adjustments

            if (stockAdjustmentType == StockAdjustmentType.SalesReturnIn)
            {
                productStock.SalesReturnQuantity += quantity;
            }

            if (stockAdjustmentType == StockAdjustmentType.SalesReturnOut)
            {
                if (productStock.SalesReturnQuantity - quantity < 0)
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new()
                        {
                            Message = "Insufficient Quantity"
                        }
                    });
                }

                productStock.SalesReturnQuantity -= quantity;
            }


            var productStockAdjustment = new ProductStockAdjustment()
            {
                WareHouseId = warehouseId,
                Quantity = quantity,
                ExpiredDate = expiredDate,
                ProductId = productId,
                UnitCost = unitCost,
                Type = stockAdjustmentType,
                Note = note
            };

            _unitOfWork.ProductStockAdjustmentRepository.Insert(productStockAdjustment);

            await _unitOfWork.SaveChanges();
        }



    }
}
