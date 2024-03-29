﻿using System.Linq;
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
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Product.Core;
using TransnationalLanka.ThreePL.Services.PurchaseOrder;
using TransnationalLanka.ThreePL.WebApi.Models.PurchaseOrder;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;
using TransnationalLanka.ThreePL.WebApi.Util.Linq;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [AuditApi(IncludeRequestBody = true)]
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IStockService _stockService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, 
            IStockService stockService, IAccountService accountService, IMapper mapper)
        {
            _purchaseOrderService = purchaseOrderService;
            _stockService = stockService;
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var user = await _accountService.GetUser(User);
            var query = _mapper.ProjectTo<PurchaseOrderBindingModel>(_purchaseOrderService.GetAll()
                .FilterByUserWareHousesOptionally(user));
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

        [HttpPost("calculate-storage")]
        public async Task<IActionResult> CalculateStorage([FromBody]CalculateStorageBindingModel model)
        {
            var totalStorage = await _stockService.CalculateStorage(new CalculateStorageByProducts()
            {
                Products = model.Products
                    .Select(p => new CalculateStorageProductItem() { ProductId = p.ProductId, Quantity = p.Quantity }).ToList()
            });

            return Ok(new
            {
                TotalStorage = totalStorage
            });
        }

        [HttpPost("mark-as-printed/{id}")]
        public async Task<IActionResult> MarkAsPrinted(long id)
        {
            await _purchaseOrderService.MarkAsPrinted(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]PurchaseOrderBindingModel model)
        {
            await _purchaseOrderService.ThrowErrorIfPurchaseOrderPrinted(model.Id);
            var updatedPurchaseOrder = await _purchaseOrderService.UpdatePurchaseOrder(_mapper.Map<PurchaseOrder>(model));
            return Ok(_mapper.Map<PurchaseOrderBindingModel>(updatedPurchaseOrder));
        }
    }
}
