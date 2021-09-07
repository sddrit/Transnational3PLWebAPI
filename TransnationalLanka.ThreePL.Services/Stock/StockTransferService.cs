using System;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Stock.Core;
using TransnationalLanka.ThreePL.Services.WareHouse;

namespace TransnationalLanka.ThreePL.Services.Stock
{
    public class StockTransferService : IStockTransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWareHouseService _wareHouseService;
        private readonly IStockService _stockService;

        public StockTransferService(IUnitOfWork unitOfWork, IWareHouseService warehouseService, IStockService stockService)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _wareHouseService = warehouseService;
        }

        public IQueryable<Dal.Entities.StockTransfer> GetAll()
        {
            return _unitOfWork.StockTransferRepository.GetAll();
        }

        public async Task<StockTransfer> AddStockTransfer(StockTransfer stockTransfer)
        {
            Guard.Argument(stockTransfer, nameof(stockTransfer)).NotNull();

            stockTransfer.Created = stockTransfer.Updated = DateTimeOffset.UtcNow;

            await ValidateStockTransfer(stockTransfer);

            await using var transaction = await _unitOfWork.GetTransaction();

            try
            {
                var fromWareHouse = await _wareHouseService.GetWareHouseById(stockTransfer.FromWareHouseId);

                if (!_wareHouseService.IsActiveWareHouse(fromWareHouse))
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = "From warehouse is not a active one"
                        }
                    });
                }

                var toWareHouse = await _wareHouseService.GetWareHouseById(stockTransfer.ToWareHouseId);

                if (!_wareHouseService.IsActiveWareHouse(toWareHouse))
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = "To warehouse is not a active one"
                        }
                    });
                }

                if (stockTransfer.ToWareHouseId == stockTransfer.FromWareHouseId)
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Message = "From and To Warehouses should not be same"
                        }
                    });
                }

                _unitOfWork.StockTransferRepository.Insert(stockTransfer);
                await _unitOfWork.SaveChanges();

                foreach (var stockTransferStockTransferItem in stockTransfer.StockTransferItems)
                {

                    await _stockService.AdjustStock(StockAdjustmentType.Out, stockTransfer.FromWareHouseId, stockTransferStockTransferItem.ProductId,
                        stockTransferStockTransferItem.UnitCost, stockTransferStockTransferItem.Quantity,
                        stockTransferStockTransferItem.ExpiredDate, $"Stock Transfer - {stockTransfer.StockTransferNumber}");

                    await _stockService.AdjustStock(StockAdjustmentType.In, stockTransfer.ToWareHouseId, stockTransferStockTransferItem.ProductId,
                        stockTransferStockTransferItem.UnitCost, stockTransferStockTransferItem.Quantity,
                        stockTransferStockTransferItem.ExpiredDate, $"Stock Transfer - {stockTransfer.StockTransferNumber}");
                }

                await transaction.CommitAsync();

                return stockTransfer;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<StockTransfer> GetStockTransferById(long id)
        {
            var stockTransfer = await _unitOfWork.StockTransferRepository.GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.StockTransferItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync();

            if (stockTransfer == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Unable to get stock transfer by id"
                    }
                });
            }

            return stockTransfer;
        }

        private async Task ValidateStockTransfer(Dal.Entities.StockTransfer stockTransfer)
        {
            var validator = new StockTransferValidator();
            var validateResult = await validator.ValidateAsync(stockTransfer);

            if (validateResult.IsValid)
            {
                return;
            }

            throw new ServiceException(validateResult.Errors.Select(e => new ErrorMessage()
            {
                Code = ErrorCodes.Model_Validation_Error_Code,
                Meta = new
                {
                    e.ErrorCode,
                    e.ErrorMessage,
                    e.PropertyName
                },
                Message = e.ErrorMessage
            }).ToArray());
        }
    }
}
