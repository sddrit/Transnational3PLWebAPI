using System.ComponentModel.DataAnnotations;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Enums.Core;
using TransnationalLanka.ThreePL.WebApi.Models.Supplier;

namespace TransnationalLanka.ThreePL.WebApi.Models.Product
{

    public class ProductBindingModel
    {
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public long SupplierId { get; set; }
        public SupplierBindingModel Supplier { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public StoringType? StoringType { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public decimal ReorderLevel { get; set; }
        [Required]
        public string Sku { get; set; }
        [Required]
        public decimal StorageUnits { get; set; }
        public decimal? Weight { get; set; }
        public MassUnit? MassUnit { get; set; }
        [Required]
        public bool Active { get; set; }
    }

    public class ProductDetailsBindingModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long SupplierId { get; set; }
        public SupplierDetailsBindingModel Supplier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StoringType? StoringType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReorderLevel { get; set; }
        public string Sku { get; set; }
        public decimal StorageUnits { get; set; }
        public decimal? Weight { get; set; }
        public MassUnit? MassUnit { get; set; }
        public bool Active { get; set; }
    }

}
