using System.ComponentModel.DataAnnotations;
using TransnationalLanka.ThreePL.WebApi.Models.Common;

namespace TransnationalLanka.ThreePL.WebApi.Models.WareHouse
{
    public class WareHouseBindingModel
    {
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public WareHouseAddressBindingModel Address { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public decimal Height { get; set; }
        [Required]
        public decimal Width { get; set; }
        [Required]
        public decimal Length { get; set; }
        public decimal StorageCapacity { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Email { get; set; }

    }

    public class WareHouseAddressBindingModel
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public CityBindingModel City { get; set; }
        public string PostalCode { get; set; }
    }

}
