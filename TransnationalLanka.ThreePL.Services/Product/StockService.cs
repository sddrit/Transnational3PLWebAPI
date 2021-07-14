using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.WareHouse;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public class StockService : IStockService
    {
        private readonly IProductService _productService;
        private readonly IWareHouseService _wareHouseService;

        private readonly IUnitOfWork _unitOfWork;

        public StockService(IProductService productService, IWareHouseService wareHouseService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public async Task AdjustStock(long warehouseId, long productId, decimal unitCost, decimal quantity, DateTime? expiredDate, string note)
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
