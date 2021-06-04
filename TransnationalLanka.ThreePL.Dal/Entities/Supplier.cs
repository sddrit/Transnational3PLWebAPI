using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Supplier : BaseEntity
    {
        public string SupplierName { get; set; }
        public string BusinessRegistrationId { get; set; }
        public string VatNumber { get; set; }
        public bool Active { get; set; }
        public SupplierAddress Address { get; set; }
        public Contact Contact { get; set; }
        public string InvoicePolicy { get; set; }
        public string ReturnPolicy { get; set; }
        public SupplierCharges SupplierCharges { get; set; }
        public virtual ICollection<Address> PickupAddress { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
