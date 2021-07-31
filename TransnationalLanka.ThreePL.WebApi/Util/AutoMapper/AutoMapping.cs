using AutoMapper;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.WebApi.Models.Account;
using TransnationalLanka.ThreePL.WebApi.Models.Common;
using TransnationalLanka.ThreePL.WebApi.Models.Delivery;
using TransnationalLanka.ThreePL.WebApi.Models.Grn;
using TransnationalLanka.ThreePL.WebApi.Models.Product;
using TransnationalLanka.ThreePL.WebApi.Models.PurchaseOrder;
using TransnationalLanka.ThreePL.WebApi.Models.Stock;
using TransnationalLanka.ThreePL.WebApi.Models.StockTransfer;
using TransnationalLanka.ThreePL.WebApi.Models.Supplier;
using TransnationalLanka.ThreePL.WebApi.Models.WareHouse;
using SupplierAddress = TransnationalLanka.ThreePL.Dal.Entities.SupplierAddress;

namespace TransnationalLanka.ThreePL.WebApi.Util.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            //Account Section
            CreateMap<User, UserBindingModel>();
            
            //Supplier
            CreateMap<City, CityBindingModel>()
                .ReverseMap();

            CreateMap<Address, AddressBindingModel>();
            CreateMap<AddressBindingModel, Address>()
                .ForMember(d => d.City, a => a.Ignore());

            CreateMap<SupplierAddress, SupplierAddressBindingModel>();
            CreateMap<SupplierAddressBindingModel, SupplierAddress>()
                .ForMember(d => d.City, a => a.Ignore());

            CreateMap<Contact, ContactBindingModel>()
                .ReverseMap();
            CreateMap<SupplierCharges, SupplierChargeBindingModel>()
                .ReverseMap();
            CreateMap<Supplier, SupplierBindingModel>()
                .ReverseMap();
            CreateMap<Supplier, SupplierDetailsBindingModel>();
            CreateMap<Product, ProductBindingModel>()
               .ReverseMap();
            CreateMap<Product, ProductDetailsBindingModel>();
            CreateMap<WareHouseAddress, WareHouseAddressBindingModel>()
            .ReverseMap();
            CreateMap<WareHouse, WareHouseBindingModel>()
             .ReverseMap();
            CreateMap<City, CityBindingModel>();

            CreateMap<PurchaseOrderItem, PurchaseOrderDetailsItemBindingModel>();
            CreateMap<PurchaseOrder, PurchaseOrderDetailsBindingModel>();
            CreateMap<PurchaseOrderItem, PurchaseOrderItemBindingModel>()
                .ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderBindingModel>()
                .ReverseMap();

            CreateMap<GoodReceivedNoteItems, GoodReceivedNoteItemsBindingModel>()
                .ReverseMap();
            CreateMap<GoodReceivedNote, GoodReceivedNoteBindingModel>()
                .ReverseMap();

            CreateMap<ProductStockAdjustment, ProductStockAdjustmentBindingModel>()
                .ReverseMap();
            CreateMap<ProductStock, ProductStockBindingModel>()
              .ReverseMap();

            CreateMap<StockTransferItem, StockTransferItemBindingModel>()
                .ReverseMap();
            CreateMap<StockTransfer, StockTransferBindingModel>()
                .ReverseMap();

            CreateMap<City, DeliveryCustomerCityBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryCustomer, DeliveryCustomerBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryItem, DeliveryItemBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryHistory, DeliveryHistoryBindingModel>()
                .ReverseMap();
            CreateMap<Delivery, DeliveryBindingModel>()
                .ReverseMap();
        }
    }
}
