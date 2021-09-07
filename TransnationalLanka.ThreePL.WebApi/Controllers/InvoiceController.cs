using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Invoice;
using TransnationalLanka.ThreePL.WebApi.Models.Invoice;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;
using System.Collections.Generic;
using System.Linq;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;
        private readonly IAccountService _accountService;

        public InvoiceController(IMapper mapper, IInvoiceService invoiceService, IAccountService accountService)
        {
            _mapper = mapper;
            _invoiceService = invoiceService;
            _accountService = accountService;
        }

        [HttpGet]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE })]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var user = await _accountService.GetUser(User);
            var invoiceQuery = _invoiceService.GetInvoices();

            if (User.IsInRole(Roles.SUPPLIER_ROLE))
            {
                invoiceQuery = invoiceQuery.Where(i => i.SupplierId == user.SupplierId);
            }

            var query = _mapper.ProjectTo<InvoiceBindingModel>(invoiceQuery);
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("{id}")]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE })]
        public async Task<IActionResult> Get(long id)
        {
            var user = await _accountService.GetUser(User);
            var invoice = await _invoiceService.GetInvoice(id);

            if (!User.IsInRole(Roles.SUPPLIER_ROLE))
            {
                return Ok(_mapper.Map<InvoiceBindingModel>(invoice));
            }

            if (invoice.SupplierId != user.SupplierId)
            {
                return Unauthorized();
            }

            return Ok(_mapper.Map<InvoiceBindingModel>(invoice));
        }

        [HttpPost("mark-as-paid")]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        public async Task<IActionResult> MarkAsPaid([FromBody] MarkAsPaidInvoiceBindingModel model)
        {
            await _invoiceService.MarkAsPaid(model.Id);
            return Ok();
        }

        [HttpPut("{id}")]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        public async Task<IActionResult> UpdateManualCharges(long id, List<InvoiceItemBindingModel> model)
        {
            var invoice =
                await _invoiceService.CreateOrUpdateManualCharges(id, model.Select(_mapper.Map<InvoiceItem>).ToList());
            return Ok(_mapper.Map<InvoiceBindingModel>(invoice));
        }
    }
}
