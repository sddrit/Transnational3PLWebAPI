using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Product;

namespace TransnationalLanka.ThreePL.Services.Grn
{
    public class GrnService : IGrnService
    {
        private readonly IStockService _stockService;
        private readonly IUnitOfWork _unitOfWork;

        public GrnService(IUnitOfWork unitOfWork, IStockService stockService)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
        }

        public IQueryable<GoodReceivedNote> GetAll()
        {
            return _unitOfWork.GoodReceiveNoteRepository.GetAll();
        }

        public async Task<GoodReceivedNote> GetById(long id)
        {
            var grn = await _unitOfWork.GoodReceiveNoteRepository.GetAll()
                .Include(g => g.GoodReceivedNoteItems)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grn == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new()
                    {
                        Message = "Unable to find grn"
                    }
                });
            }

            return grn;
        }

        public async Task<GoodReceivedNote> AddGoodReceivedNote(GoodReceivedNote goodReceivedNote)
        {
            await using var transaction = await _unitOfWork.GetTransaction();

            try
            {

                _unitOfWork.GoodReceiveNoteRepository.Insert(goodReceivedNote);
                await _unitOfWork.SaveChanges();

                foreach (var item in goodReceivedNote.GoodReceivedNoteItems)
                {
                    if (goodReceivedNote.Type == GrnType.Received)
                    {
                        await _stockService.AdjustStock(goodReceivedNote.WareHouseId, item.ProductId, item.UnitCost,
                            item.Quantity,
                            item.ExpiredDate,
                            $"Good Received - GRN #{goodReceivedNote.Id}");
                    }
                    else
                    {
                        await _stockService.AdjustStock(goodReceivedNote.WareHouseId, item.ProductId, item.UnitCost,
                            -item.Quantity,
                            item.ExpiredDate,
                            $"Good Return - GRN #{goodReceivedNote.Id}");
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return goodReceivedNote;

        }


    }
}
