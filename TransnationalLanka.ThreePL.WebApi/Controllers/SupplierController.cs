using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Supplier;
using TransnationalLanka.ThreePL.WebApi.Models.Supplier;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupplierService _supplierService;
        private readonly IStockService _stockService;
        private readonly IAccountService _accountService;

        public SupplierController(IMapper mapper, ISupplierService supplierService, 
            IStockService stockService, IAccountService accountService)
        {
            _mapper = mapper;
            _supplierService = supplierService;
            _accountService = accountService;
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<SupplierListItemBindingModel>(_supplierService.GetSuppliers());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var supplier = await _supplierService.GetSupplierById(id);
            return Ok(_mapper.Map<SupplierBindingModel>(supplier));
        }

        [HttpGet("storage-details/{id}")]
        public async Task<IActionResult> GetStorageDetails(long id)
        {
            var supplier = await _supplierService.GetSupplierById(id);
            var totalStorage = await _stockService.GetTotalStorage(id);
            var remainStorage = await _stockService.GetRemainStorage(id);
            var totalStorageByWareHouses = await _stockService.GetTotalStorageByWareHouses(id);
            return Ok(new
            {
                AllocatedStorage = supplier.SupplierCharges.AllocatedUnits,
                TotalStorage = totalStorage,
                RemainStorage = remainStorage,
                TotalStorageByWareHouses = totalStorageByWareHouses
            });
        }

        [HttpPost("set-status")]
        public async Task<IActionResult> Post([FromBody]SetSupplierStatus model)
        {
            await _supplierService.SetSupplierStatus(model.Id, model.Status);
            return Ok();
        }

        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateSupplierAccount model)
        {
            await _accountService.CreateUser(
                new User()
                {
                    UserName = model.UserName, Active = true, Email = model.Email, SupplierId = model.SupplierId
                }, model.Password, Roles.SUPPLIER_ROLE);
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
