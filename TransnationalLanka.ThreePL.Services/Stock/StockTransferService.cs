using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Stock.Core;

namespace TransnationalLanka.ThreePL.Services.Stock
{
    public interface IStockTransferService
    {
        IQueryable<Dal.Entities.StockTransfer> GetAll();
        Task<StockTransfer> AddStockTransfer(StockTransfer stockTransfer);
        Task<StockTransfer> GetStockTransferById(long id);
    }

    public class StockTransferService : IStockTransferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockTransferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Dal.Entities.StockTransfer> GetAll()
        {
            return _unitOfWork.StockTransferRepository.GetAll();
        }

        public async Task<StockTransfer> AddStockTransfer(StockTransfer stockTransfer)
        {
            Guard.Argument(stockTransfer, nameof(stockTransfer)).NotNull();

            await ValidateStockTransfer(stockTransfer);

            _unitOfWork.StockTransferRepository.Insert(stockTransfer);
            await _unitOfWork.SaveChanges();
            return stockTransfer;
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
