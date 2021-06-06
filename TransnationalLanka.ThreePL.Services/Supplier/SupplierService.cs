using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Common.Mapper;
using TransnationalLanka.ThreePL.Services.Supplier.Core;

namespace TransnationalLanka.ThreePL.Services.Supplier
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Dal.Entities.Supplier> GetSuppliers()
        {
            return _unitOfWork.SupplierRepository.GetAll()
                .Include(s => s.Address.City);
        }

        public async Task<Dal.Entities.Supplier> AddSupplier(Dal.Entities.Supplier supplier)
        {
            Guard.Argument(supplier, nameof(supplier)).NotNull();

            await ValidateSupplier(supplier);

            _unitOfWork.SupplierRepository.Insert(supplier);
            await _unitOfWork.SaveChanges();

            //Todo send the create new supplier to tracking application

            return await GetSupplierById(supplier.Id);
        }

        public async Task<Dal.Entities.Supplier> UpdateSupplier(Dal.Entities.Supplier supplier)
        {
            Guard.Argument(supplier, nameof(supplier)).NotNull();

            await ValidateSupplier(supplier);

            var currentSupplier = await GetSupplierById(supplier.Id);

            var mapper = ServiceMapper.GetMapper();
            mapper.Map(supplier, currentSupplier);

            await _unitOfWork.SaveChanges();

            //Todo send the update supplier details to tracking application

            return await GetSupplierById(supplier.Id);
        }

        public async Task<Dal.Entities.Supplier> GetSupplierById(long id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetAll()
                .Include(s => s.Address)
                .ThenInclude(a => a.City)
                .Include(s => s.PickupAddress)
                .ThenInclude(pa => pa.City)
                .Where(s => s.Id == id)              
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"Unable to find supplier by id {id}"
                    }
                });
            }

            return supplier;
        }

        public async Task SetSupplierStatus(long id, bool status)
        {
            var currentSupplier = await GetSupplierById(id);
            currentSupplier.Active = status;
            await _unitOfWork.SaveChanges();

            //Todo send the status change to tracking application
        }

        public bool IsActiveSupplier(Dal.Entities.Supplier supplier)
        {
            return supplier.Active;
        }

        private async Task ValidateSupplier(Dal.Entities.Supplier supplier)
        {
            var supplierValidator = new SupplierValidator();
            var validateResult = await supplierValidator.ValidateAsync(supplier);

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
