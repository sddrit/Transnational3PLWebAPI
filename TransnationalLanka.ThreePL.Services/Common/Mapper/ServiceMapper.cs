using AutoMapper;
using AutoMapper.EquivalencyExpression;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Common.Mapper
{
    public static class ServiceMapper
    {
        private static MapperConfiguration _mapperConfiguration;

        static ServiceMapper()
        {
            _mapperConfiguration = new MapperConfiguration(configuration =>
            {
                configuration.AddCollectionMappers();
                configuration.CreateMap<Address, Address>()
                    .ForMember(d => d.SupplierId, o => o.Ignore())
                    .ForMember(d => d.Supplier, o => o.Ignore())
                    .ForMember(d => d.City, o => o.Ignore())

                    .EqualityComparison(((s, d) => s.Id == d.Id));
                configuration.CreateMap<Dal.Entities.SupplierAddress, Dal.Entities.SupplierAddress>()
                    .ForMember(d => d.City, o => o.Ignore());
                configuration.CreateMap<Dal.Entities.Supplier, Dal.Entities.Supplier>()
                    .ForMember(d => d.Products, o => o.Ignore())
                    .ForMember(d => d.Users, o => o.Ignore())
                    .EqualityComparison((s, d) => s.Id == d.Id);

                configuration.CreateMap<WareHouseAddress, WareHouseAddress>()
                    .ForMember(d => d.City, o => o.Ignore());
                configuration.CreateMap<Dal.Entities.WareHouse, Dal.Entities.WareHouse>();
                configuration.CreateMap<Dal.Entities.Product, Dal.Entities.Product>()
                    .ForMember(d => d.Supplier, o => o.Ignore());

                configuration.CreateMap<PurchaseOrderItem, PurchaseOrderItem>()
                    .ForMember(i => i.Product, o => o.Ignore())
                    .ForMember(i => i.PurchaseOrderId, o => o.Ignore())
                    .ForMember(i => i.PurchaseOrder, o => o.Ignore())
                    .EqualityComparison((s, d) => s.Id == d.Id);
                configuration.CreateMap<Dal.Entities.PurchaseOrder, Dal.Entities.PurchaseOrder>()
                    .ForMember(p => p.WareHouse, o => o.Ignore())
                    .ForMember(p => p.Supplier, o => o.Ignore())
                    .EqualityComparison((s, d) => s.Id == d.Id);
            });

            _mapperConfiguration.CompileMappings();
        }

        public static IMapper GetMapper()
        {
            return _mapperConfiguration.CreateMapper();
        }
    }
}
