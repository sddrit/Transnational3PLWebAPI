using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.ApiCredential;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.WebApi.Models.ApiCredentail;
using TransnationalLanka.ThreePL.WebApi.Models.Supplier;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCredentialController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IApiCredentialService _apiCredentialService;

        public ApiCredentialController(IMapper mapper, IApiCredentialService apiCredentialService)
        {
            _mapper = mapper;
            _apiCredentialService = apiCredentialService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(_apiCredentialService.Get(), loadOptions);
        }       

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ApiCredentailBindingModel model)
        {
            var supplier = await _apiCredentialService.AddApiCredentail(_mapper.Map<ApiCredential>(model));
            return Ok(_mapper.Map<ApiCredentailBindingModel>(supplier));
        }

       
    }
}
