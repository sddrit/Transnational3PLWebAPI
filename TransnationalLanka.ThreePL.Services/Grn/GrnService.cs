using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.PurchaseOrder;

namespace TransnationalLanka.ThreePL.Services.Grn
{
    public class GrnService : IGrnService
    {
        private readonly IStockService _stockService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IUnitOfWork _unitOfWork;

        public GrnService(IUnitOfWork unitOfWork, IStockService stockService, 
            IPurchaseOrderService purchaseOrderService)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _purchaseOrderService = purchaseOrderService;
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

        public async Task<GoodReceivedNote> GetByIdIncludeWithProduct(long id)
        {
            var grn = await _unitOfWork.GoodReceiveNoteRepository.GetAll()
                .Include(g => g.GoodReceivedNoteItems)
                .ThenInclude(n => n.Product)
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
                goodReceivedNote.Created = DateTimeOffset.UtcNow;
                goodReceivedNote.Updated = DateTimeOffset.UtcNow;
                
                _unitOfWork.GoodReceiveNoteRepository.Insert(goodReceivedNote);
                await _unitOfWork.SaveChanges();

                foreach (var item in goodReceivedNote.GoodReceivedNoteItems)
                {
                    if (goodReceivedNote.Type == GrnType.Received)
                    {
                        await _stockService.AdjustStock(StockAdjustmentType.In, goodReceivedNote.WareHouseId, item.ProductId, item.UnitCost,
                            item.Quantity,
                            item.ExpiredDate,
                            $"Good Received - GRN #{goodReceivedNote.Id}");
                    }
                    else if(goodReceivedNote.Type == GrnType.Return)
                    {
                        await _stockService.AdjustStock(StockAdjustmentType.Out, goodReceivedNote.WareHouseId, item.ProductId, item.UnitCost,
                            item.Quantity,
                            item.ExpiredDate,
                            $"Good Return - GRN #{goodReceivedNote.Id}");
                    }
                    else
                    {
                        await _stockService.AdjustStock(StockAdjustmentType.SalesReturnIn, goodReceivedNote.WareHouseId, item.ProductId, item.UnitCost,
                            item.Quantity,
                            item.ExpiredDate,
                            $"Sales Return - GRN #{goodReceivedNote.Id}");
                    }
                }

                if (goodReceivedNote.PurchaseOrderId.HasValue)
                {
                    var purchaseOrder =
                        await _purchaseOrderService.GetPurchaseOrderById(goodReceivedNote.PurchaseOrderId.Value);

                    foreach (var goodReceivedNoteItem in goodReceivedNote.GoodReceivedNoteItems)
                    {
                        var purchaseOrderItem =
                            purchaseOrder.PurchaseOrderItems.FirstOrDefault(poi =>
                                poi.ProductId == goodReceivedNoteItem.ProductId);

                        if (purchaseOrderItem != null && goodReceivedNote.Type == GrnType.Received)
                        {
                            purchaseOrderItem.ReceivedQuantity += goodReceivedNoteItem.Quantity;
                        }
                        else if (purchaseOrderItem != null && goodReceivedNote.Type == GrnType.Return)
                        {
                            purchaseOrderItem.ReceivedQuantity -= goodReceivedNoteItem.Quantity;
                        }
                    }

                    var purchaseOrderStatus = _purchaseOrderService.GetPurchaseOrderStatus(purchaseOrder);
                    purchaseOrder.Status = purchaseOrderStatus;

                    await _purchaseOrderService.UpdatePurchaseOrder(purchaseOrder);
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
