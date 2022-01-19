using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.ClientApi.Util.Authorization;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Product;

namespace TransnationalLanka.ThreePL.ClientApi.Controllers
{
    [Authorize(AuthenticationSchemes = SupplierAuthOptions.DefaultScemeName)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IAccountService _accountService;

        public ProductsController(IAccountService accountService,
            IProductService productService)
        {
            _accountService = accountService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var user = await _accountService.GetUser(User);
            return Ok(new
            {
                Username = user.UserName
            });
        }
    }
}
