using System;
using System.Collections.Generic;
using System.Linq;
using TransnationalLanka.ThreePL.Core.Enums;
using TransnationalLanka.ThreePL.Core.Enums.Core;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Metadata.Core;

namespace TransnationalLanka.ThreePL.Services.Metadata
{
    public interface IMetadataService
    {
        List<StoreTypeMetadataItem> GetStoreTypes();
        List<DistanceUnitMetadataItem> GetDistanceUnits();
        List<MassUnitMetadataItem> GetMassUnit();
        List<StockAdjustmentTypeMetadataItem> GetStockAdjustmentTypes();
        List<GrnTypeMetadataItem> GetGrnTypes();
        List<DeliveryStatusMetadataItem> GetDeliveryStatus();
        List<DeliveryTypeMetadata> GetDeliveryTypes();
        List<PurchaseOrderStatusMetadataItem> GetPurchaseOrderStatus();
        List<DeliveryTrackingStatusMetadataItem> GetDeliveryTrackingStatus();
        List<TaxTypeMetadataItem> GetTaxTypes();
    }

    public class MetadataService : IMetadataService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MetadataService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<StoreTypeMetadataItem> GetStoreTypes()
        {
            return Enum.GetValues(typeof(StoringType)).Cast<StoringType>().Select(e => new StoreTypeMetadataItem()
            {
                Id = (int) e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<DistanceUnitMetadataItem> GetDistanceUnits()
        {
            return Enum.GetValues(typeof(DistanceUnit)).Cast<DistanceUnit>().Select(e => new DistanceUnitMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<MassUnitMetadataItem> GetMassUnit()
        {
            return Enum.GetValues(typeof(MassUnit)).Cast<MassUnit>().Select(e => new MassUnitMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<StockAdjustmentTypeMetadataItem> GetStockAdjustmentTypes()
        {
            return Enum.GetValues(typeof(StockAdjustmentType)).Cast<StockAdjustmentType>().Select(e => new StockAdjustmentTypeMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<GrnTypeMetadataItem> GetGrnTypes()
        {
            return Enum.GetValues(typeof(GrnType)).Cast<GrnType>().Select(e => new GrnTypeMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<DeliveryStatusMetadataItem> GetDeliveryStatus()
        {
            return Enum.GetValues(typeof(DeliveryStatus)).Cast<DeliveryStatus>().Select(e => new DeliveryStatusMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<DeliveryTypeMetadata> GetDeliveryTypes()
        {
            return Enum.GetValues(typeof(DeliveryType)).Cast<DeliveryType>().Select(e => new DeliveryTypeMetadata()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<PurchaseOrderStatusMetadataItem> GetPurchaseOrderStatus()
        {
            return Enum.GetValues(typeof(PurchaseOrderStatus)).Cast<PurchaseOrderStatus>().Select(e => new PurchaseOrderStatusMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<DeliveryTrackingStatusMetadataItem> GetDeliveryTrackingStatus()
        {
            return Enum.GetValues(typeof(TrackingStatus)).Cast<TrackingStatus>().Select(e => new DeliveryTrackingStatusMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }

        public List<TaxTypeMetadataItem> GetTaxTypes()
        {
            return Enum.GetValues(typeof(TaxType)).Cast<TaxType>().Select(e => new TaxTypeMetadataItem()
            {
                Id = (int)e,
                Name = e.GetDescription()
            }).ToList();
        }
    }
}
