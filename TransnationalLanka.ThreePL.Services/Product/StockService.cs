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
                .SumAsync(s => (s.Quantity + s.ReturnQuantity) * s.Product.StorageUnits);
        }

        public async Task<List<TotalStorageByWareHouse>> GetTotalStorageByWareHouses(long supplierId)
        {
            return await _unitOfWork.ProductStockRepository.GetAll()
                .Include(s => s.WareHouse)
                .Include(s => s.Product)
                .Where(s => s.Product.SupplierId == supplierId)
                .Select(s => new { s.WareHouseId, s.WareHouse.Name, s.WareHouse.Code, s.Product.StorageUnits, s.Quantity, s.ReturnQuantity })
                .GroupBy(s => new {s.WareHouseId, s.Code, s.Name})
                .Select(g => new TotalStorageByWareHouse()
                {
                    WareHouseId = g.Key.WareHouseId,
                    WareHouseCode = g.Key.Code,
                    WareHouseName = g.Key.Name,
                    TotalStorage = g.Sum(i => (i.Quantity* i.StorageUnits) + (i.ReturnQuantity * i.StorageUnits))
                }).ToListAsync();
        }

        public async Task<decimal> CalculateStorage(CalculateStorageByProducts calculateStorageByProducts)
        {
            var totalStorage = 0m;

            foreach(var product in calculateStorageByProducts.Products)
            {
                totalStorage += await _unitOfWork.ProductRepository.GetAll()
                    .Where(p => p.Id == product.ProductId)
                    .Select(p => p.StorageUnits * product.Quantity)
                    .FirstOrDefaultAsync();
            }

            return totalStorage;
        }

        public async Task TransferReturnStock(long warehouseId, long productId, decimal unitCost, decimal quantity, DateTime? expiredDate,
            string note)
        {
            await AdjustReturnStock(warehouseId, productId, unitCost, quantity, expiredDate, note);
            await AdjustStock(warehouseId, productId, unitCost, -quantity, expiredDate, note);
        }

        public async Task AdjustReturnStock(long warehouseId, long productId, decimal unitCost, decimal quantity, DateTime? expiredDate,
            string note)
        {
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
                    Quantity = quantity,
                    WareHouseId = warehouseId
                };

                _unitOfWork.ProductStockRepository.Insert(productStock);
            }
            else
            {
                if (quantity < 0 && (productStock.ReturnQuantity + quantity) < 0)
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new()
                        {
                            Message = "Insufficient return quantity to adjust"
                        }
                    });
                }

                productStock.ReturnQuantity += quantity;
            }

            var productStockAdjustment = new ProductStockAdjustment()
            {
                WareHouseId = warehouseId,
                Quantity = quantity,
                ExpiredDate = expiredDate,
                ProductId = productId,
                UnitCost = unitCost,
                Type = quantity < 0 ? StockAdjustmentType.ReturnOut : StockAdjustmentType.ReturnIn,
                Note = note
            };

            _unitOfWork.ProductStockAdjustmentRepository.Insert(productStockAdjustment);
            await _unitOfWork.SaveChanges();
        }

        public async Task AdjustStock(long warehouseId, long productId, decimal unitCost, decimal quantity, DateTime? expiredDate, 
            string note)
        {
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
                    Quantity = quantity,
                    WareHouseId = warehouseId
                };

                _unitOfWork.ProductStockRepository.Insert(productStock);
            }
            else
            {
                if (quantity < 0 && (productStock.Quantity + quantity) < 0)
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new()
                        {
                            Message = "Insufficient Quantity"
                        }
                    });
                }

                productStock.Quantity += quantity;
            }

            var productStockAdjustment = new ProductStockAdjustment()
            {
                WareHouseId = warehouseId,
                Quantity = quantity,
                ExpiredDate = expiredDate,
                ProductId = productId,
                UnitCost = unitCost,
                Type = quantity < 0 ? StockAdjustmentType.Out : StockAdjustmentType.In,
                Note = note
            };

            _unitOfWork.ProductStockAdjustmentRepository.Insert(productStockAdjustment);

            await _unitOfWork.SaveChanges();
        }
    }
}
