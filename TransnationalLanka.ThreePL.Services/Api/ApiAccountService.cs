using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Core.Util.Api;
using TransnationalLanka.ThreePL.Dal;

namespace TransnationalLanka.ThreePL.Services.Api
{
    public class ApiAccountService : IApiAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApiAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Dal.Entities.Account> CreateAccount(long supplierId)
        {
            if (await ExitsAccount(supplierId))
            {
                throw new ServiceException(new[]
                {
                    new ErrorMessage()
                    {
                        Message = "Supplier already have an account"
                    }
                });
            }

            var account = new Dal.Entities.Account()
            {
                SupplierId = supplierId,
                Active = true,
                Secret = ApiExtensions.GenerateSecret(),
                ClientId = ApiExtensions.GenerateClientId(supplierId)
            };

            _unitOfWork.AccountRepository.Insert(account);
            await _unitOfWork.SaveChanges();

            return account;
        }

        public async Task<Dal.Entities.Account> GetAccountByClientIdAndSecret(string clientId, string secret)
        {
            var account = await _unitOfWork.AccountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.ClientId == clientId && a.Secret == secret);

            if (account == null)
            {
                throw new ServiceException(new[]
                {
                    new ErrorMessage()
                    {
                        Code = ErrorCodes.API_INVALID_CREDENTIALS_ERROR_CODE,
                        Message = "Invalid credentials"
                    }
                });
            }

            if (!account.Active)
            {
                throw new ServiceException(new[]
                {
                    new ErrorMessage()
                    {
                        Code = ErrorCodes.API_INVALID_CREDENTIALS_ERROR_CODE,
                        Message = "Account is deactivated"
                    }
                });
            }

            return account;
        }

        public async Task<Dal.Entities.Account> GetAccountBySupplierId(long supplierId)
        {
            var account = await _unitOfWork.AccountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.SupplierId == supplierId && a.Active == true);

            if (account == null)
            {
                throw new ServiceException(new ErrorMessage[]{ new ErrorMessage()
                {
                    Message = "Unable to find account"
                }});
            }

            return account;
        }

        public async Task DeleteAccount(long supplierId)
        {
            var account = await _unitOfWork.AccountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.SupplierId == supplierId);

            if (account == null)
            {
                throw new ServiceException(new ErrorMessage[]{ new ErrorMessage()
                {
                    Message = "Unable to find account"
                }});
            }

            _unitOfWork.AccountRepository.Delete(account.Id);
            await _unitOfWork.SaveChanges();
        }

        public async Task<Dal.Entities.Account> SetAccountStatus(long supplierId, bool status)
        {
            var account = await _unitOfWork.AccountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.SupplierId == supplierId);

            if (account == null)
            {
                throw new ServiceException(new ErrorMessage[]{ new ErrorMessage()
                {
                    Message = "Unable to find account"
                }});
            }

            account.Active = status;
            await _unitOfWork.SaveChanges();
            return account;
        }

        public async Task<bool> ExitsAccount(long supplierId)
        {
            return await _unitOfWork.AccountRepository.GetAll()
                .AnyAsync(a => a.SupplierId == supplierId && a.Active);
        }

    }
}
