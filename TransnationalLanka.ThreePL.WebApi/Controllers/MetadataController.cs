using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Services.Metadata;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService _metadataService;

        public MetadataController(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var storeTypes = _metadataService.GetStoreTypes();
            var massUnits = _metadataService.GetMassUnit();
            var distanceUnits = _metadataService.GetDistanceUnits();
            var stockAdjustmentTypes = _metadataService.GetStockAdjustmentTypes();
            return Ok(new
            {
                StoreTypes = storeTypes,
                MassUnits = massUnits,
                DistanceUnits = distanceUnits,
                StockAdjustmentTypes = stockAdjustmentTypes
            });
        }
    }
}
