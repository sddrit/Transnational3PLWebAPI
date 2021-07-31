using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Delivery : BaseEntity
    {
        public DeliveryType Type { get; set; }
        public string DeliveryNo { get; set; }
        public long SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public long? WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public DeliveryCustomer DeliveryCustomer { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ICollection<DeliveryItem> DeliveryItems { get; set; }
        public string[] TrackingNumbers { get; set; }
        [NotMapped]
        public decimal SubTotal { get; set; }
        public ICollection<DeliveryHistory> DeliveryHistories { get; set; }
    }

    public class DeliveryItem : BaseEntity
    {
        public long DeliveryId { get; set; }
        public virtual Delivery Delivery { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        [NotMapped] public decimal Value => Quantity * UnitCost;
    }

    [Owned]
    public class DeliveryCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public virtual City City { get; set; }
        public string PostalCode { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }

        [NotMapped] public string FullName => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public string Address
        {
            get
            {
                var addressLines = new List<string>();

                if (!string.IsNullOrEmpty(AddressLine1))
                {
                    addressLines.Add(AddressLine1);
                }

                if (!string.IsNullOrEmpty(AddressLine2))
                {
                    addressLines.Add(AddressLine2);
                }

                if (string.IsNullOrEmpty(City.CityName))
                {
                    addressLines.Add(City.CityName);
                }

                if (string.IsNullOrEmpty(PostalCode))
                {
                    addressLines.Add(PostalCode);
                }

                return string.Join(", ", addressLines);
            }
        }
    }

    public class DeliveryHistory : BaseEntity
    {
        public long DeliveryId { get; set; }
        public virtual Delivery Delivery { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
    }
}
