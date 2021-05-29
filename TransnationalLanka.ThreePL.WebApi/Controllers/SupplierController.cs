using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.WebApi.Models.Supplier;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    //[ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupplierService _supplierService;

        public SupplierController(IMapper mapper, ISupplierService supplierService)
        {
            _mapper = mapper;
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(_supplierService.GetSuppliers(), loadOptions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var supplier = await _supplierService.GetSupplierById(id);
            return Ok(_mapper.Map<SupplierBindingModel>(supplier));
        }

        [HttpPost("set-status")]
        public async Task<IActionResult> Post([FromBody]SetSupplierStatus model)
        {
            await _supplierService.SetSupplierStatus(model.Id, model.Status);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SupplierBindingModel model)
        {
            var supplier = await _supplierService.AddSupplier(_mapper.Map<Supplier>(model));
            return Ok(_mapper.Map<SupplierBindingModel>(supplier));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SupplierBindingModel model)
        {
            var supplier = await _supplierService.UpdateSupplier(_mapper.Map<Supplier>(model));
            return Ok(_mapper.Map<SupplierBindingModel>(supplier));
        }
    }
}
