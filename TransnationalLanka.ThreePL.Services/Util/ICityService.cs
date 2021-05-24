using System.Linq;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Util
{
    public interface ICityService
    {
        IQueryable<City> GetCities();
    }
}