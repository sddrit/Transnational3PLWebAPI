using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.WebApi.Models.Product;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<ProductDetailsBindingModel>(_productService.GetProducts());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(_mapper.Map<ProductBindingModel>(product));
        }

        [HttpPost("set-status")]
        public async Task<IActionResult> Post([FromBody]SetProductStatus model)
        {
            await _productService.SetProductStatus(model.Id, model.Status);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ProductBindingModel model)
        {
            var product = await _productService.AddProduct(_mapper.Map<Product>(model));
            return Ok(_mapper.Map<ProductBindingModel>(product));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductBindingModel model)
        {
            var product = await _productService.UpdateProduct(_mapper.Map<Product>(model));
            return Ok(_mapper.Map<ProductBindingModel>(product));
        }
    }
}
