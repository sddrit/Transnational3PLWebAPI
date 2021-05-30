using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.WebApi.Models.Common;

namespace TransnationalLanka.ThreePL.WebApi.Models.Product
{

    public class ProductBindingModel
    {
        public long Id { get; set; }
        [Required]
        public long SupplierId { get; set; }
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

}
