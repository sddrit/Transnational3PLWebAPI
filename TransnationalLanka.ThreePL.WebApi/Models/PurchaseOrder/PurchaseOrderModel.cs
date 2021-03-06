using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.WebApi.Models.Product;
using TransnationalLanka.ThreePL.WebApi.Models.Supplier;
using TransnationalLanka.ThreePL.WebApi.Models.WareHouse;

namespace TransnationalLanka.ThreePL.WebApi.Models.PurchaseOrder
{
    public class PurchaseOrderDetailsBindingModel
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public bool Printed { get; set; }
        public DateTimeOffset? PrintedDate { get; set; }
        public string PoNumber { get; set; }
        public long SupplierId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public SupplierDetailsBindingModel Supplier { get; set; }
        public long? WareHouseId { get; set; }
        public WareHouseBindingModel WareHouse { get; set; }
        public ICollection<PurchaseOrderDetailsItemBindingModel> PurchaseOrderItems { get; set; }
    }

    public class PurchaseOrderDetailsItemBindingModel
    {
        public long Id { get; set; }
        public long PurchaseOrderId { get; set; }
        public long ProductId { get; set; }
        public ProductDetailsBindingModel Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Value { get; set; }
    }

    public class PurchaseOrderBindingModel
    {
        public long Id { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public string PoNumber { get; set; }
        public string Note { get; set; }
        public long SupplierId { get; set; }
        public long? WareHouseId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public ICollection<PurchaseOrderItemBindingModel> PurchaseOrderItems { get; set; }
    }

    public class PurchaseOrderItemBindingModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class CalculateStorageBindingModel
    {
        [Required]
        public List<CalculateStorageProductItemBindingModel> Products { get; set; }
    }

    public class CalculateStorageProductItemBindingModel
    {
        [Required]
        public long ProductId { get; set; }
        [Required]
        public decimal Quantity { get; set; }
    }
}
