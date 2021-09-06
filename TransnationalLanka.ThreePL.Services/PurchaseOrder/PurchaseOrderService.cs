using System;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Common.Mapper;
using TransnationalLanka.ThreePL.Services.PurchaseOrder.Core;

namespace TransnationalLanka.ThreePL.Services.PurchaseOrder
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Dal.Entities.PurchaseOrder> GetAll()
        {
            return _unitOfWork.PurchaseOrdeRepository.GetAll();
        }

        public async Task<Dal.Entities.PurchaseOrder> AddPurchaseOrder(Dal.Entities.PurchaseOrder purchaseOrder)
        {
            Guard.Argument(purchaseOrder, nameof(purchaseOrder)).NotNull();

            purchaseOrder.Status = PurchaseOrderStatus.Pending;

            await ValidatePurchaseOrder(purchaseOrder);

            purchaseOrder.Created = DateTimeOffset.UtcNow;
            purchaseOrder.Updated = DateTimeOffset.UtcNow;

            _unitOfWork.PurchaseOrdeRepository.Insert(purchaseOrder);
            await _unitOfWork.SaveChanges();
            return purchaseOrder;
        }

        public async Task<Dal.Entities.PurchaseOrder> UpdatePurchaseOrder(Dal.Entities.PurchaseOrder purchaseOrder)
        {
            Guard.Argument(purchaseOrder, nameof(purchaseOrder)).NotNull();

            await ValidatePurchaseOrder(purchaseOrder);

            var currentPurchaseOrder = await GetPurchaseOrderById(purchaseOrder.Id);

            if (currentPurchaseOrder.Printed)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "You can't update printed purchase order"
                    }
                });
            }

            var mapper = ServiceMapper.GetMapper();
            mapper.Map(purchaseOrder, currentPurchaseOrder);

            currentPurchaseOrder.Updated = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChanges();
            return currentPurchaseOrder;
        }

        public async Task<Dal.Entities.PurchaseOrder> MarkAsPrinted(long id)
        {
            var purchaseOrder = await GetPurchaseOrderById(id);

            purchaseOrder.Printed = true;
            purchaseOrder.PrintedDate = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChanges();

            return purchaseOrder;
        }

        public async Task<Dal.Entities.PurchaseOrder> SetPurchaseOrderStatus(long purchaseOrderId, PurchaseOrderStatus status)
        {
            var purchaseOrder = await GetPurchaseOrderById(purchaseOrderId);
            purchaseOrder.Status = status;
            await _unitOfWork.SaveChanges();
            return purchaseOrder;
        }

        public async Task<Dal.Entities.PurchaseOrder> GetPurchaseOrderById(long id)
        {
            var purchaseOrder = await _unitOfWork.PurchaseOrdeRepository.GetAll()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                .ThenInclude(i => i.Product)
                .Include(p => p.WareHouse)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchaseOrder == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Unable to get purchase order by id"
                    }
                });
            }

            return purchaseOrder;
        }

        public PurchaseOrderStatus GetPurchaseOrderStatus(Dal.Entities.PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null)
            {
                throw new ArgumentNullException(nameof(purchaseOrder));
            }

            if (purchaseOrder.PurchaseOrderItems.All(poi => poi.ReceivedQuantity >= poi.Quantity))
            {
                return PurchaseOrderStatus.Received;
            }

            if (purchaseOrder.PurchaseOrderItems.All(poi => poi.ReceivedQuantity == 0))
            {
                return PurchaseOrderStatus.Pending;
            }

            return PurchaseOrderStatus.PartiallyReceived;
        }

        private async Task ValidatePurchaseOrder(Dal.Entities.PurchaseOrder purchaseOrder)
        {
            var validator = new PurchaseOrderValidator();
            var validateResult = await validator.ValidateAsync(purchaseOrder);

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
