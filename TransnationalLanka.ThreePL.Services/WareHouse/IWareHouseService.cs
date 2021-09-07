using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Services.WareHouse.Core;

namespace TransnationalLanka.ThreePL.Services.WareHouse
{
    public interface IWareHouseService
    {
        IQueryable<Dal.Entities.WareHouse> GetWareHouses();
        Task<List<WareHouseStorageInfo>> GetStorageDetails();
        Task<Dal.Entities.WareHouse> AddWareHouse(Dal.Entities.WareHouse warehouse);
        Task<Dal.Entities.WareHouse> UpdateWareHouse(Dal.Entities.WareHouse warehouse);
        Task<Dal.Entities.WareHouse> GetWareHouseById(long id);
        Task SetWareHouseStatus(long id, bool status);
        bool IsActiveWareHouse(Dal.Entities.WareHouse wareHouse);
    }
}