using System.Linq;
using TransnationalLanka.ThreePL.Dal.Core;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.WebApi.Util.Linq
{
    public static class WareHouseLinqExtensions
    {
        public static IQueryable<T> FilterByUserWareHouses<T>(this IQueryable<T> query, User user) where T: IWareHouseRelatedEntity
        {
            var userWareHouseIds = user.UserWareHouses.Select(w => w.WareHouseId).ToList();

            if (!userWareHouseIds.Any())
            {
                return query;
            }

            return query.Where(q => userWareHouseIds.Contains(q.WareHouseId));

        }

        public static IQueryable<T> FilterByUserWareHousesOptionally<T>(this IQueryable<T> query, User user) where T : IOptionallyWareHouseRelatedEntity
        {
            var userWareHouseIds = user.UserWareHouses.Select(w => w.WareHouseId).ToList();

            if (!userWareHouseIds.Any())
            {
                return query;
            }

            return query.Where(q => q.WareHouseId == null || (q.WareHouseId.HasValue && userWareHouseIds.Contains(q.WareHouseId.Value)));

        }
    }
}
