using System;
using System.ComponentModel.DataAnnotations;

namespace TransnationalLanka.ThreePL.WebApi.Models.Stock
{
    public class TransferStockBindingModel
    {
        [Required] 
        public string TransferType { get; set; }
        [Required]
        public long WareHouseId { get; set; }
        [Required] 
        public long ProductId { get; set; }
        [Required] 
        public decimal UnitCost { get; set; }
        public DateTime? ExpiredDate { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required] 
        public decimal DamageQuantity { get; set; }
        [Required] 
        public string TrackingNumber { get; set; }
        [Required]
        public string Note { get; set; }
    }
}
