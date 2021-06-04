using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Util
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<City> GetCities()
        {
            return _unitOfWork.CityRepository.GetAll();
        }

        public async Task<City> GetCityById(long id)
        {
            var city = await _unitOfWork.CityRepository.GetAll()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = "Unable to find city"
                    }
                });
            }

            return city;
        }
    }
}
