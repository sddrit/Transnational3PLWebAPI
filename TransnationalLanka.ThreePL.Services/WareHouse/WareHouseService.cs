using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Common.Mapper;
using TransnationalLanka.ThreePL.Services.WareHouse.Core;

namespace TransnationalLanka.ThreePL.Services.WareHouse
{
    public class WareHouseService : IWareHouseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WareHouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Dal.Entities.WareHouse> GetWareHouses()
        {
            return _unitOfWork.WareHouseRepository.GetAll()
                .Include(w => w.Address.City);
        }

        public async Task<List<WareHouseStorageInfo>> GetStorageDetails()
        {
            var wareHouseStorageInfo = await _unitOfWork.WareHouseRepository.GetAll()
                .Include(w => w.ProductStocks)
                .ThenInclude(s => s.Product)
                .Where(w => w.Active)
                .Select(w => new WareHouseStorageInfo()
                {
                    Code = w.Code,
                    WareHouseId = w.Id,
                    WareHouseName = w.Name,
                    UsedSpace = w.ProductStocks.Sum(s =>
                        (s.DamageStockQuantity + s.DispatchReturnQuantity + s.Quantity + s.SalesReturnQuantity) *
                        (s.Product.Height * s.Product.Length * s.Product.Width)),
                    StorageCapacity = w.Height * w.Length * w.Width
                }).ToListAsync();

            return wareHouseStorageInfo;
        }

        public async Task<Dal.Entities.WareHouse> AddWareHouse(Dal.Entities.WareHouse warehouse)
        {
            Guard.Argument(warehouse, nameof(warehouse)).NotNull();

            await ValidateWareHouse(warehouse);

            if (_unitOfWork.WareHouseRepository.GetAll().Any(w => w.Code.ToLower() == warehouse.Code.ToLower()))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = $"{warehouse.Code} is already exits"
                    }
                });
            }

            _unitOfWork.WareHouseRepository.Insert(warehouse);
            await _unitOfWork.SaveChanges();

            return warehouse;
        }

        public async Task<Dal.Entities.WareHouse> UpdateWareHouse(Dal.Entities.WareHouse warehouse)
        {
            Guard.Argument(warehouse, nameof(warehouse)).NotNull();

            await ValidateWareHouse(warehouse);

            var currentWareHouse = await GetWareHouseById(warehouse.Id);

            if (_unitOfWork.WareHouseRepository.GetAll().Any(w => w.Code.ToLower() == warehouse.Code.ToLower()
                                                                  && w.Id != warehouse.Id))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = $"{warehouse.Code} is already exits"
                    }
                });
            }

            var mapper = ServiceMapper.GetMapper();
            mapper.Map(warehouse, currentWareHouse);

            await _unitOfWork.SaveChanges();

            return currentWareHouse;
        }

        public async Task<Dal.Entities.WareHouse> GetWareHouseById(long id)
        {
            var warehouse = await _unitOfWork.WareHouseRepository.GetAll()
                .Include(w => w.Address.City)
                .Where(w => w.Id == id)                
                .FirstOrDefaultAsync();

            if (warehouse == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"Unable to find warehouse by id {id}"
                    }
                });
            }

            return warehouse;
        }

        public async Task SetWareHouseStatus(long id, bool status)
        {
            var currentWarehouse = await GetWareHouseById(id);
            currentWarehouse.Active = status;
            await _unitOfWork.SaveChanges();

            //Todo send the status change to tracking application
        }

        public bool IsActiveWareHouse(Dal.Entities.WareHouse wareHouse)
        {
            return wareHouse.Active;
        }

        private async Task ValidateWareHouse(Dal.Entities.WareHouse warehouse)
        {
            var warehouseValidator = new WareHouseValidator();
            var validateResult = await warehouseValidator.ValidateAsync(warehouse);

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
