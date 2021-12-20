using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;

namespace TransnationalLanka.ThreePL.Services.Api
{
    public class ApiAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApiAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<Dal.Entities.Account> CreateAccount(long supplierId)
        //{

        //}

        //public async Task<Dal.Entities.Account> GetAccountBySupplierId(long supplierId)
        //{
        //    var account = await _unitOfWork.AccountRepository.GetAll()
        //        .FirstOrDefaultAsync(a => a.SupplierId == supplierId);

        //    if (account == null)
        //    {
        //        throw new ServiceException("Unable to find account");
        //    }

        //    return account;
        //}

    }
}
