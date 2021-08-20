using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransnationalLanka.ThreePL.WebApi.Models.Common;

namespace TransnationalLanka.ThreePL.WebApi.Models.Supplier
{
    public class ContactBindingModel
    {
        public string ContactPerson { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }

    public class SupplierChargeBindingModel
    {
        [Required]
        public decimal AllocatedUnits { get; set; }
        [Required]
        public decimal StorageChargePerUnit { get; set; }
        [Required]
        public decimal AdditionalChargePerUnitPrice { get; set; }
        [Required]
        public decimal HandlingCharge { get; set; }
    }

    public class SupplierListItemBindingModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string SupplierName { get; set; }
        public string BusinessRegistrationId { get; set; }
        public string VatNumber { get; set; }
        public bool Active { get; set; }
    }

    public class SupplierBindingModel
    {
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string SupplierName { get; set; }
        public string BusinessRegistrationId { get; set; }
        public string VatNumber { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public SupplierAddressBindingModel Address { get; set; }
        [Required]
        public ContactBindingModel Contact { get; set; }
        public string InvoicePolicy { get; set; }
        public string ReturnPolicy { get; set; }
        [Required]
        public SupplierChargeBindingModel SupplierCharges { get; set; }
        [Required]
        public virtual ICollection<AddressBindingModel> PickupAddress { get; set; }
    }

    public class SupplierDetailsBindingModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string SupplierName { get; set; }
        public string BusinessRegistrationId { get; set; }
        public string VatNumber { get; set; }
        public bool Active { get; set; }
        public SupplierAddressBindingModel Address { get; set; }
        public ContactBindingModel Contact { get; set; }
    }

}
