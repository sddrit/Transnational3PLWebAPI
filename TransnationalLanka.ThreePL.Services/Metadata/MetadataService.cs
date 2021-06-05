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

    }
}
