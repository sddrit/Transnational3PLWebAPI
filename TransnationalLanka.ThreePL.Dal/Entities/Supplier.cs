using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Supplier : BaseEntity
    {
        public string SupplierName { get; set; }
        public string BusinessRegistrationId { get; set; }
        public string VatNumber { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string MobileNumber { get; set; }
        public Address Address { get; set; }
        public Contact Contact { get; set; }
        public string InvoicePolicy { get; set; }
        public string ReturnPolicy { get; set; }
        public SupplierCharges SupplierCharges { get; set; }
        public virtual ICollection<Address> PickupAddress { get; set; }
    }
}
