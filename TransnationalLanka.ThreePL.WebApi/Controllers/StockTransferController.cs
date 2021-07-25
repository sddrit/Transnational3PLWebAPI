using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Stock;
using TransnationalLanka.ThreePL.WebApi.Models.StockTransfer;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransferController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStockTransferService _stockTransferService;

        public StockTransferController(IMapper mapper, IStockTransferService stockTransferService)
        {
            _mapper = mapper;
            _stockTransferService = stockTransferService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<StockTransferBindingModel>(_stockTransferService.GetAll());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("get-stock-transfer/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var stockTransfer = await _stockTransferService.GetStockTransferById(id);
            return Ok(_mapper.Map<StockTransferBindingModel>(stockTransfer));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StockTransferBindingModel model)
        {
            return Ok(_mapper.Map<StockTransferBindingModel>(
                await _stockTransferService.AddStockTransfer(_mapper.Map<StockTransfer>(model))));
        }

    }
}
