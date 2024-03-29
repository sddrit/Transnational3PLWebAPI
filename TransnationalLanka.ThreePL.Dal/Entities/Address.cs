﻿namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Address : BaseEntity
    {
        public long? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public virtual City City { get; set; }
        public string PostalCode { get; set; }
    }
}
