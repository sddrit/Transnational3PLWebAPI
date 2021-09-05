using System.Linq;
using AutoMapper;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.WebApi.Models.Account;
using TransnationalLanka.ThreePL.WebApi.Models.Common;
using TransnationalLanka.ThreePL.WebApi.Models.Delivery;
using TransnationalLanka.ThreePL.WebApi.Models.Grn;
using TransnationalLanka.ThreePL.WebApi.Models.Invoice;
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
            CreateMap<User, UserBindingModel>()
                .ForMember(u => u.WareHouses,
                    o => o.MapFrom(u => u.UserWareHouses.Select(uw => uw.WareHouseId).ToArray()));

            //City Mapping
            CreateMap<City, CityBindingModel>()
                .ReverseMap();

            //Address Mapping
            CreateMap<Address, AddressBindingModel>();
            CreateMap<AddressBindingModel, Address>()
                .ForMember(d => d.City, a => a.Ignore());

            //Supplier Mapping
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
            CreateMap<Supplier, SupplierListItemBindingModel>()
                .ReverseMap();


            //Product Mapping
            CreateMap<Product, ProductBindingModel>()
               .ReverseMap();
            CreateMap<Product, ProductDetailsBindingModel>();
            CreateMap<UnitOfMeasure, UnitOfMeasureBindingModel>()
                .ReverseMap();

            //WareHouse Mapping
            CreateMap<WareHouseAddress, WareHouseAddressBindingModel>()
            .ReverseMap();
            CreateMap<WareHouse, WareHouseBindingModel>()
             .ReverseMap();


            //Purchase Order Mapping
            CreateMap<PurchaseOrderItem, PurchaseOrderDetailsItemBindingModel>();
            CreateMap<PurchaseOrder, PurchaseOrderDetailsBindingModel>();
            CreateMap<PurchaseOrderItem, PurchaseOrderItemBindingModel>()
                .ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderBindingModel>()
                .ReverseMap();

            //GRN Mapping
            CreateMap<GoodReceivedNoteItems, GoodReceivedNoteItemsBindingModel>()
                .ReverseMap();
            CreateMap<GoodReceivedNote, GoodReceivedNoteBindingModel>()
                .ReverseMap();

            //Product Stock Mapping
            CreateMap<ProductStockAdjustment, ProductStockAdjustmentBindingModel>()
                .ReverseMap();
            CreateMap<ProductStock, ProductStockBindingModel>()
              .ReverseMap();


            //ST Mapping
            CreateMap<StockTransferItem, StockTransferItemBindingModel>()
                .ReverseMap();
            CreateMap<StockTransfer, StockTransferBindingModel>()
                .ReverseMap();

            //Delivery Mapping
            CreateMap<City, DeliveryCustomerCityBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryCustomer, DeliveryCustomerBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryItem, DeliveryItemBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryHistory, DeliveryHistoryBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryTracking, DeliveryTrackingBindingModel>()
                .ReverseMap();
            CreateMap<DeliveryTrackingItem, DeliveryTrackingItemBindingModel>()
                .ReverseMap();
            CreateMap<Delivery, DeliveryBindingModel>()
                .ReverseMap();
            CreateMap<Delivery, DeliveryListItemBindingModel>()
                .ReverseMap();


            //Invoice Mapping
            CreateMap<InvoiceItem, InvoiceItemBindingModel>()
                .ReverseMap();
            CreateMap<Invoice, InvoiceBindingModel>()
                .ReverseMap();
        }
    }
}
