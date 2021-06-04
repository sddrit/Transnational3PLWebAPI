using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Util
{
    public interface ICityService
    {
        IQueryable<City> GetCities();
        Task<City> GetCityById(long id);
    }
}