using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.WebApi.Models.Product;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly IAccountService _accountService;

        public ProductController(IMapper mapper, IProductService productService, IAccountService accountService)
        {
            _mapper = mapper;
            _productService = productService;
            _accountService = accountService;
        }

        [HttpGet]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            IQueryable<ProductDetailsBindingModel> query;

            if (User.IsInRole(Roles.SUPPLIER_ROLE))
            {
                var user = await _accountService.GetUser(User);
                query = _mapper.ProjectTo<ProductDetailsBindingModel>(_productService.GetProducts(user.SupplierId ?? 0));
                return await DataSourceLoader.LoadAsync(query, loadOptions);
            }

            query = _mapper.ProjectTo<ProductDetailsBindingModel>(_productService.GetProducts());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("{id}")]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE,
            Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
        public async Task<IActionResult> Get(long id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(_mapper.Map<ProductBindingModel>(product));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost("set-status")]
        public async Task<IActionResult> Post([FromBody]SetProductStatus model)
        {
            await _productService.SetProductStatus(model.Id, model.Status);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ProductBindingModel model)
        {
            var product = await _productService.AddProduct(_mapper.Map<Product>(model));
            return Ok(_mapper.Map<ProductBindingModel>(product));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductBindingModel model)
        {
            var product = await _productService.UpdateProduct(_mapper.Map<Product>(model));
            return Ok(_mapper.Map<ProductBindingModel>(product));
        }
    }
}
