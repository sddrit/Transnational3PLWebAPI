using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.PurchaseOrder;
using TransnationalLanka.ThreePL.WebApi.Models.PurchaseOrder;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IMapper _mapper;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IMapper mapper)
        {
            _purchaseOrderService = purchaseOrderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<PurchaseOrderBindingModel>(_purchaseOrderService.GetAll());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderById(id);
            return Ok(_mapper.Map<PurchaseOrderDetailsBindingModel>(purchaseOrder));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PurchaseOrderBindingModel model)
        {
            var createdPurchaseOrder = await _purchaseOrderService.AddPurchaseOrder(_mapper.Map<PurchaseOrder>(model));
            return Ok(_mapper.Map<PurchaseOrderBindingModel>(createdPurchaseOrder));
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromBody]PurchaseOrderBindingModel model)
        {
            var updatedPurchaseOrder = await _purchaseOrderService.UpdatePurchaseOrder(_mapper.Map<PurchaseOrder>(model));
            return Ok(_mapper.Map<PurchaseOrderBindingModel>(updatedPurchaseOrder));
        }
    }
}
