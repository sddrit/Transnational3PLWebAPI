using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using TransnationalLanka.ThreePL.Dal.Entities;
using DevExtreme.AspNet.Mvc;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Delivery;
using TransnationalLanka.ThreePL.WebApi.Models.Delivery;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IMapper _mapper;

        public DeliveryController(IMapper mapper, IDeliveryService deliveryService)
        {
            _mapper = mapper;
            _deliveryService = deliveryService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<DeliveryBindingModel>(_deliveryService.GetDeliveries());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("get-delivery/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var delivery = await _deliveryService.GetDeliveryById(id);
            return Ok(_mapper.Map<DeliveryBindingModel>(delivery));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeliveryBindingModel model)
        {
            return Ok(_mapper.Map<DeliveryBindingModel>(await _deliveryService.CreateDelivery(_mapper.Map<Delivery>(model))));
        }

        [HttpPost("mark-as-processing")]
        public async Task<IActionResult> Post([FromBody] MarkAsProcessingBindingModel model)
        {
            await _deliveryService.MarkAsProcessing(model.DeliveryId, model.RequiredTrackingNumberCount);
            return Ok();
        }

        [HttpPost("mark-as-dispatch")]
        public async Task<IActionResult> Post([FromBody] MarkAsDispatchBindingModel model)
        {
            await _deliveryService.MarkAsDispatch(model.DeliveryId, model.WarehouseId);
            return Ok();
        }

        [HttpPost("mark-as-complete")]
        public async Task<IActionResult> Post([FromBody] MarkAsCompleteBindingModel model)
        {
            await _deliveryService.MarkAsComplete(model.DeliveryId);
            return Ok();
        }

        [HttpPost("mark-as-return")]
        public async Task<IActionResult> Post([FromBody] MarkAsReturnBindingModel model)
        {
            await _deliveryService.MarkAsReturn(model.DeliveryId, model.Note);
            return Ok();
        }

        [HttpPost("mark-as-customer-return")]
        public async Task<IActionResult> Post([FromBody] MarkAsCustomerReturnBindingModel model)
        {
            await _deliveryService.MarkAsCustomerReturn(model.DeliveryId, model.Note);
            return Ok();
        }
    }
}
