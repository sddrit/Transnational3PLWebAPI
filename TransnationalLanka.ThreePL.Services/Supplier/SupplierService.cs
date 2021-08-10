using System;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Integration.Tracker.Exceptions;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;
using TransnationalLanka.ThreePL.Services.Common.Mapper;
using TransnationalLanka.ThreePL.Services.Supplier.Core;

namespace TransnationalLanka.ThreePL.Services.Supplier
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TrackerApiService _trackerApiService;

        public SupplierService(IUnitOfWork unitOfWork, TrackerApiService trackerApiService)
        {
            _unitOfWork = unitOfWork;
            _trackerApiService = trackerApiService;
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

            await using var transaction = await _unitOfWork.GetTransaction();

            try
            {
                _unitOfWork.SupplierRepository.Insert(supplier);
                await _unitOfWork.SaveChanges();

                var savedSupplier = await GetSupplierById(supplier.Id);

                var response = await _trackerApiService.CreateCustomer(new CreateCustomerRequest()
                {
                    CustomerName = savedSupplier.SupplierName,
                    CustomerAddress =
                        $"{savedSupplier.Address.AddressLine1} {savedSupplier.Address.AddressLine2} {savedSupplier.Address.City.CityName} {savedSupplier.Address.PostalCode}",
                    ContactNumber = savedSupplier.Contact.Phone,
                    EmailAddress = savedSupplier.Contact.Email,
                    NicNo = string.Empty,
                    ContactPerson = savedSupplier.Contact.ContactPerson,
                    TaxRegNo = savedSupplier.VatNumber,
                    Vat = string.IsNullOrEmpty(savedSupplier.VatNumber) ? "0" : "1",
                    Remarks = string.Empty,
                    CreatedDate = System.DateTime.Now,
                    TplSupplierCode = savedSupplier.Code
                });

                if (response.IsSuccess != "1")
                {
                    throw new ServiceException(new ErrorMessage[]
                    {
                        new ErrorMessage()
                        {
                            Code = string.Empty,
                            Message = $"Unable to create a supplier in tracker side"
                        }
                    });
                }

                savedSupplier.TrackerCode = response.Result.CustomerCode;
                await _unitOfWork.SaveChanges();

                await transaction.CommitAsync();

            }
            catch (TrackingApiException ex)
            {
                await transaction.RollbackAsync();
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"Unable to create a supplier in tracker side"
                    }
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return await GetSupplierById(supplier.Id);
        }

        public async Task<Dal.Entities.Supplier> UpdateSupplier(Dal.Entities.Supplier supplier)
        {
            Guard.Argument(supplier, nameof(supplier)).NotNull();

            await ValidateSupplier(supplier);

            await using var transaction = await _unitOfWork.GetTransaction();

            try
            {
                var currentSupplier = await GetSupplierById(supplier.Id);
                supplier.TrackerCode = currentSupplier.TrackerCode;

                var mapper = ServiceMapper.GetMapper();
                mapper.Map(supplier, currentSupplier);

                await _unitOfWork.SaveChanges();

                var savedSupplier = await GetSupplierById(supplier.Id);

                await _trackerApiService.UpdateCustomerStatus(new UpdateCustomerRequest()
                {
                    Active = supplier.Active ? "1" : "0",
                    CustomerCode = savedSupplier.TrackerCode,
                    StatusDate = DateTime.Now,
                    StatusReason = "Update the supplier"
                });

                await transaction.CommitAsync();
            }
            catch (TrackingApiException ex)
            {
                await transaction.RollbackAsync();
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"Unable to create a supplier in tracker side"
                    }
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

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
