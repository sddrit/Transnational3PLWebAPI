using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using TransnationalLanka.ThreePL.Dal.Entities;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Http;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Delivery;
using TransnationalLanka.ThreePL.WebApi.Models.Delivery;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;
using TransnationalLanka.ThreePL.WebApi.Util.Linq;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public DeliveryController(IMapper mapper, IDeliveryService deliveryService, IAccountService accountService)
        {
            _mapper = mapper;
            _deliveryService = deliveryService;
            _accountService = accountService;
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE, 
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            IQueryable<DeliveryListItemBindingModel> query = null;
            var user = await _accountService.GetUser(User);

            if (User.IsInRole(Roles.SUPPLIER_ROLE))
            {
                query = _mapper.ProjectTo<DeliveryListItemBindingModel>(_deliveryService.GetDeliveries(user.SupplierId ?? 0));
                return await DataSourceLoader.LoadAsync(query, loadOptions);
            }

            query = _mapper.ProjectTo<DeliveryListItemBindingModel>(_deliveryService.GetDeliveries()
                .FilterByUserWareHousesOptionally(user));
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpGet("delivery-stat")]
        public async Task<IActionResult> GetDeliveryStat()
        {
            var user = await _accountService.GetUser(User);

            if (User.IsInRole(Roles.SUPPLIER_ROLE))
            {
                return Ok(new
                {
                    DayStat = await _deliveryService.GetTodayDeliveryStat(user.SupplierId.Value),
                    MonthlyStat = await _deliveryService.GetMonthlyDeliveryStat(user.SupplierId.Value)
                });
            }

            var wareHousesIds = user.UserWareHouses.Select(w => w.WareHouseId).ToArray();

            return Ok(new
            {
                DayStat = await _deliveryService.GetTodayDeliveryStat(wareHouses: wareHousesIds),
                MonthlyStat = await _deliveryService.GetMonthlyDeliveryStat(wareHouses: wareHousesIds)
            });
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpGet("get-tracking-details/{trackingNumber}")]
        public async Task<IActionResult> GetTrackingDetails(string trackingNumber)
        {
            return Ok(await _deliveryService.GetTrackingDetails(trackingNumber));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpGet("latest-delivery-unit-price/{productId}")]
        public async Task<IActionResult> GetLatestDeliveryUnitPrice(long productId)
        {
            var unitPrice = await _deliveryService.GetLatestDeliveryUnitPrice(productId);
            return Ok(new
            {
                UnitPrice = unitPrice
            });
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpGet("get-delivery/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var delivery = await _deliveryService.GetDeliveryById(id);
            return Ok(_mapper.Map<DeliveryBindingModel>(delivery));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeliveryBindingModel model)
        {
            return Ok(_mapper.Map<DeliveryBindingModel>(await _deliveryService.CreateDelivery(_mapper.Map<Delivery>(model))));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] DeliveryBindingModel model)
        {
            return Ok(_mapper.Map<DeliveryBindingModel>(await _deliveryService.UpdateDelivery(_mapper.Map<Delivery>(model))));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPut("map-delivery-product")]
        public async Task<IActionResult> MapDeliveryProduct([FromBody] DeliveryBindingModel model)
        {
            return Ok(_mapper.Map<DeliveryBindingModel>(await _deliveryService.MapDeliveryProduct(_mapper.Map<Delivery>(model))));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPost("mark-as-processing")]
        public async Task<IActionResult> Post([FromBody] MarkAsProcessingBindingModel model)
        {
            await _deliveryService.MarkAsProcessing(model.DeliveryId, model.RequiredTrackingNumberCount);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPost("mark-as-dispatch")]
        public async Task<IActionResult> Post([FromBody] MarkAsDispatchBindingModel model)
        {
            await _deliveryService.MarkAsDispatch(model.DeliveryId, model.WarehouseId);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPost("mark-as-complete")]
        public async Task<IActionResult> Post([FromBody] MarkAsCompleteBindingModel model)
        {
            await _deliveryService.MarkAsComplete(model.DeliveryId, model.TrackingNumbers);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPost("mark-as-return")]
        public async Task<IActionResult> Post([FromBody] MarkAsReturnBindingModel model)
        {
            await _deliveryService.MarkAsReturn(model.DeliveryId, model.TrackingNumbers, model.Note);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpPost("process-delivery-sheet")]
        public async Task<IActionResult> ProcessDeliveryComplete(IFormFile file)
        {
            var fileStream = new MemoryStream();
            await file.CopyToAsync(fileStream);

            var result = await _deliveryService.ProcessDeliverySheet(fileStream);
            return Ok(result);
        }
    }
}
