using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Audit.WebApi;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.WebApi.Models.Stock;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;
using TransnationalLanka.ThreePL.WebApi.Util.Linq;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [AuditApi(IncludeRequestBody = true)]
    [Route("api/[controller]")]
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
        Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStockService _stockService;
        private readonly IAccountService _accountService;

        public StockController(IMapper mapper, IStockService stockService, IAccountService accountService)
        {
            _mapper = mapper;
            _stockService = stockService;
            _accountService = accountService;
        }

        [HttpGet("get-stocks-by-product-id/{id}")]
        public async Task<LoadResult> GetStocksByProductId(long id, DataSourceLoadOptions loadOptions)
        {
            var user = await _accountService.GetUser(User);

            var query = _mapper.ProjectTo<ProductStockBindingModel>(_stockService
                .GetStocksByProductId(id).FilterByUserWareHouses(user));
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("get-stock-adjustments-by-product-id/{id}")]
        public async Task<LoadResult> GetStockAdjustmentsBtProductId(long id, DataSourceLoadOptions loadOptions)
        {
            var user = await _accountService.GetUser(User);
            var query = _mapper.ProjectTo<ProductStockAdjustmentBindingModel>(_stockService
                .GetStockAdjustmentsByProductId(id).FilterByUserWareHouses(user));
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpPost("transfer-stock")]
        public async Task<IActionResult> TransferReturnStock([FromBody] TransferStockBindingModel model)
        {
            switch (model.TransferType.ToLower())
            {
                case "dispatch return":
                    await _stockService.TransferDispatchReturnStock(model.WareHouseId, model.ProductId, model.UnitCost, model.Quantity,
                        model.DamageQuantity, model.ExpiredDate, model.Note, model.TrackingNumber);
                    break;
                case "sales return":
                    await _stockService.TransferSalesReturnStock(model.WareHouseId, model.ProductId, model.UnitCost, model.Quantity,
                        model.DamageQuantity, model.ExpiredDate, model.Note, model.TrackingNumber);
                    break;
            }
            return Ok();
        }
    }
}
