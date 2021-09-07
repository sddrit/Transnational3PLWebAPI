using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Grn;
using TransnationalLanka.ThreePL.WebApi.Models.Grn;
using DevExtreme.AspNet.Mvc;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;
using TransnationalLanka.ThreePL.WebApi.Util.Linq;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE,
        Roles.USER_ROLE, Roles.WAREHOUSE_MANAGER_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class GrnController : ControllerBase
    {
        private readonly IGrnService _grnService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public GrnController(IMapper mapper, IGrnService grnService, IAccountService accountService)
        {
            _mapper = mapper;
            _grnService = grnService;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var user = await _accountService.GetUser(User);
            var query = _mapper.ProjectTo<GoodReceivedNoteBindingModel>(_grnService.GetAll()
                .FilterByUserWareHouses(user));
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var product = await _grnService.GetById(id);
            return Ok(_mapper.Map<GoodReceivedNoteBindingModel>(product));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GoodReceivedNoteBindingModel model)
        {
            var response = await _grnService.AddGoodReceivedNote(_mapper.Map<GoodReceivedNote>(model));
            return Ok(_mapper.Map<GoodReceivedNoteBindingModel>(response));
        }

    }
}
