using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Api;
using TransnationalLanka.ThreePL.WebApi.Models.Api;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IApiAccountService _apiAccountService;
        private readonly IMapper _mapper;

        public SettingsController(IAccountService accountService,
            IApiAccountService apiAccountService,
            IMapper mapper)
        {
            _accountService = accountService;
            _apiAccountService = apiAccountService;
            _mapper = mapper;
        }

        [ThreePlAuthorize(new[] { Roles.SUPPLIER_ROLE })]
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount()
        {
            var user = await _accountService.GetUser(User);
            var account = await _apiAccountService.CreateAccount(user.SupplierId.Value);
            return Ok(_mapper.Map<ApiAccount>(account));

        }

        [ThreePlAuthorize(new[] { Roles.SUPPLIER_ROLE })]
        [HttpGet("get-account")]
        public async Task<IActionResult> GetAccount()
        {
            var user = await _accountService.GetUser(User);

            if (!await _apiAccountService.ExitsAccount(user.SupplierId.Value))
            {
                return NotFound();
            }

            var account = await _apiAccountService.GetAccountBySupplierId(user.SupplierId.Value);
            return Ok(_mapper.Map<ApiAccount>(account));
        }

        [ThreePlAuthorize(new[] { Roles.SUPPLIER_ROLE })]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _accountService.GetUser(User);
            await _apiAccountService.DeleteAccount(user.SupplierId.Value);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.SUPPLIER_ROLE })]
        [HttpPost("set-status")]
        public async Task<IActionResult> DeleteAccount([FromBody]ApiAccountStatusBindingModel model)
        {
            var user = await _accountService.GetUser(User);
            var account = await _apiAccountService.SetAccountStatus(user.SupplierId.Value, model.Status);
            return Ok(_mapper.Map<ApiAccount>(account));
        }
    }
}
