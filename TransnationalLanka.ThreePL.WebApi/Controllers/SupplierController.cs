using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.WebApi.Models.Account;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
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
        public async Task<IActionResult> Get(long id)
        {
            var supplier = await _supplierService.GetSupplierById(id);
            return Ok(_mapper.Map<SupplierBindingModel>(supplier));
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
