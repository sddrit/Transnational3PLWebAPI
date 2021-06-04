using Microsoft.EntityFrameworkCore;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class WareHouse : BaseEntity
    {
        public string Name { get; set; }        
        public WareHouseAddress Address { get; set; }
        public bool Active { get; set; }

    }

    [Owned]
    public class WareHouseAddress
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public virtual City City { get; set; }
        public string PostalCode { get; set; }

    }
}
