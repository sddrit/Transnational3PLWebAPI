using System.Linq;
using System.Threading.Tasks;
using Audit.WebApi;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.WareHouse;
using TransnationalLanka.ThreePL.WebApi.Models.WareHouse;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [AuditApi(IncludeRequestBody = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWareHouseService _warehouseService;
        private readonly IAccountService _accountService;

        public WareHouseController(IMapper mapper, IWareHouseService warehouseService, IAccountService accountService)
        {
            _mapper = mapper;
            _warehouseService = warehouseService;
            _accountService = accountService;
        }

        [HttpGet]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var user = await _accountService.GetUser(User);
            var userWareHouseIds = user.UserWareHouses.Select(w => w.WareHouseId).ToList();

            var wareHouseQuery = _warehouseService.GetWareHouses();

            if (userWareHouseIds.Any())
            {
                wareHouseQuery = wareHouseQuery.Where(w => userWareHouseIds.Contains(w.Id));
            }

            return await DataSourceLoader.LoadAsync(wareHouseQuery, loadOptions);
        }

        [HttpGet("warehouse-storage-info")]
        public async Task<IActionResult> GetWareHouseStorageInfo()
        {
            var storageInfo = await _warehouseService.GetStorageDetails();
            return Ok(storageInfo);
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var warehouse = await _warehouseService.GetWareHouseById(id);
            return Ok(_mapper.Map<WareHouseBindingModel>(warehouse));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost("set-status")]
        public async Task<IActionResult> Post([FromBody]SetWareHouseStatus model)
        {
            await _warehouseService.SetWareHouseStatus(model.Id, model.Status);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]WareHouseBindingModel model)
        {
            var warehouse = await _warehouseService.AddWareHouse(_mapper.Map<WareHouse>(model));
            return Ok(_mapper.Map<WareHouseBindingModel>(warehouse));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] WareHouseBindingModel model)
        {
            var warehouse = await _warehouseService.UpdateWareHouse(_mapper.Map<WareHouse>(model));
            return Ok(_mapper.Map<WareHouseBindingModel>(warehouse));
        }
    }
}
