using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string MobileNumber { get; set; }
        public virtual List<Address> PickupAddress { get; set; }
    }
}
