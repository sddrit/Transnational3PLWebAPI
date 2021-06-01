using AutoMapper;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.WebApi.Models.Account;
using TransnationalLanka.ThreePL.WebApi.Models.Common;
using TransnationalLanka.ThreePL.WebApi.Models.Product;
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
            CreateMap<Address, AddressBindingModel>()
                .ReverseMap();
            CreateMap<SupplierAddress, SupplierAddressBindingModel>()
                .ReverseMap();
            CreateMap<Contact, ContactBindingModel>()
                .ReverseMap();
            CreateMap<SupplierCharges, SupplierChargeBindingModel>()
                .ReverseMap();
            CreateMap<Supplier, SupplierBindingModel>()
                .ReverseMap();
            CreateMap<Product, ProductBindingModel>()
               .ReverseMap();
            CreateMap<WareHouseAddress, WareHouseAddressBindingModel>()
            .ReverseMap();
            CreateMap<WareHouse, WareHouseBindingModel>()
             .ReverseMap();
        }
    }
}
