using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Metadata;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.WebApi.Models.Product;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService _metadataService;
        private readonly IProductService _productService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public MetadataController(IMapper mapper, IMetadataService metadataService, 
            IProductService productService, IAccountService accountService)
        {
            _mapper = mapper;
            _metadataService = metadataService;
            _productService = productService;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var storeTypes = _metadataService.GetStoreTypes();
            var massUnits = _metadataService.GetMassUnit();
            var distanceUnits = _metadataService.GetDistanceUnits();
            var stockAdjustmentTypes = _metadataService.GetStockAdjustmentTypes();
            var grnTypes = _metadataService.GetGrnTypes();
            var deliveryStatus = _metadataService.GetDeliveryStatus();
            var deliveryTypes = _metadataService.GetDeliveryTypes();
            var unitOfMeasures = await _productService.GetUnitOfMeasures();
            var purchaseOrderStatus = _metadataService.GetPurchaseOrderStatus();
            var deliveryTrackingStatus = _metadataService.GetDeliveryTrackingStatus();
            var roles = (await _accountService.GetRoles()).ToList();

            return Ok(new
            {
                StoreTypes = storeTypes,
                MassUnits = massUnits,
                DistanceUnits = distanceUnits,
                StockAdjustmentTypes = stockAdjustmentTypes,
                GrnTypes = grnTypes,
                DeliveryStatus = deliveryStatus,
                DeliveryTypes = deliveryTypes,
                PurchaseOrderStatus = purchaseOrderStatus,
                UnitOfMeasures = unitOfMeasures.Select(_mapper.Map<UnitOfMeasureBindingModel>),
                TrackingStatus = deliveryTrackingStatus,
                Roles = roles
            });
        }
    }
}
