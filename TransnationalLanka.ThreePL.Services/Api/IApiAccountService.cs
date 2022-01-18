using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Api
{
    public interface IApiAccountService
    {
        Task<Dal.Entities.Account> CreateAccount(long supplierId);
        Task<Dal.Entities.Account> GetAccountByClientIdAndSecret(string clientId, string secret);
        Task<Dal.Entities.Account> GetAccountBySupplierId(long supplierId);
        Task<bool> ExitsAccount(long supplierId);
        Task DeleteAccount(long supplierId);
        Task<Dal.Entities.Account> SetAccountStatus(long supplierId, bool status);
    }
}