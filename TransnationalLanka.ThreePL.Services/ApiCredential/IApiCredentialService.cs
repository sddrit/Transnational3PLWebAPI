using System.Linq;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.ApiCredential
{
    public interface IApiCredentialService
    {
        IQueryable<Dal.Entities.ApiCredential> Get();
        Task<Dal.Entities.ApiCredential> AddApiCredentail(Dal.Entities.ApiCredential apiCredential);
   
    }
}