using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.WebApi.Models.Stock;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStockService _stockService;

        public StockController(IMapper mapper, IStockService stockService)
        {
            _mapper = mapper;
            _stockService = stockService;
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE })]
        [HttpGet("get-stocks-by-product-id/{id}")]
        public async Task<LoadResult> GetStocksByProductId(long id, DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<ProductStockBindingModel>( _stockService.GetStocksByProductId(id));
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE })]
        [HttpGet("get-stock-adjustments-by-product-id/{id}")]
        public async Task<LoadResult> GetStockAdjustmentsBtProductId(long id, DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<ProductStockAdjustmentBindingModel>( _stockService.GetStockAdjustmentsByProductId(id));
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }
    }
}
