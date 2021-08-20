using System;
using System.Collections.Generic;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.WebApi.Models.Product;
using TransnationalLanka.ThreePL.WebApi.Models.Supplier;
using TransnationalLanka.ThreePL.WebApi.Models.WareHouse;

namespace TransnationalLanka.ThreePL.WebApi.Models.Delivery
{
    public class DeliveryListItemBindingModel
    {
        public long Id { get; set; }
        public DeliveryType Type { get; set; }
        public string DeliveryNo { get; set; }
        public long SupplierId { get; set; }
        public long? WareHouseId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public DateTime DeliveryDate { get; set; }
    }

    public class DeliveryBindingModel
    {
        public long Id { get; set; }
        public DeliveryType Type { get; set; }
        public string DeliveryNo { get; set; }
        public long SupplierId { get; set; }
        public long? WareHouseId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public virtual WareHouseBindingModel WareHouse { get; set; }
        public virtual SupplierBindingModel Supplier { get; set; }
        public DeliveryCustomerBindingModel DeliveryCustomer { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ICollection<DeliveryItemBindingModel> DeliveryItems { get; set; }
        public string[] TrackingNumbers { get; set; }
        public ICollection<DeliveryHistoryBindingModel> DeliveryHistories { get; set; }
    }

    public class DeliveryItemBindingModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public ProductBindingModel Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Value => Quantity * UnitCost;
    }

    public class DeliveryCustomerBindingModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public DeliveryCustomerCityBindingModel City { get; set; }
        public string PostalCode { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
    }

    public class DeliveryHistoryBindingModel
    {
        public string UserName { get; set; }
        public string Note { get; set; }
    }

    public class DeliveryCustomerCityBindingModel
    {
        public long Id { get; set; }
        public string CityName { get; set; }
    }

    public class MarkAsProcessingBindingModel
    {
        public long DeliveryId { get; set; }
        public int RequiredTrackingNumberCount { get; set; }
    }

    public class MarkAsDispatchBindingModel
    {
        public long DeliveryId { get; set; }
        public long WarehouseId { get; set; }
    }

    public class MarkAsCompleteBindingModel
    {
        public long DeliveryId { get; set; }
    }

    public class MarkAsReturnBindingModel
    {
        public long DeliveryId { get; set; }
        public string Note { get; set; }
    }

    public class MarkAsCustomerReturnBindingModel
    {
        public long DeliveryId { get; set; }
        public string Note { get; set; }
    }

}
