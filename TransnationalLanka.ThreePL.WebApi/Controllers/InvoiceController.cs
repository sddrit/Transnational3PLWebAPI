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

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IMapper mapper, IInvoiceService invoiceService)
        {
            _mapper = mapper;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<InvoiceBindingModel>(_invoiceService.GetInvoices());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var invoice = await _invoiceService.GetInvoice(id);
            return Ok(_mapper.Map<InvoiceBindingModel>(invoice));
        }

        [HttpPost("mark-as-paid")]
        public async Task<IActionResult> MarkAsPaid([FromBody] MarkAsPaidInvoiceBindingModel model)
        {
            await _invoiceService.MarkAsPaid(model.Id);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManualCharges(long id, List<InvoiceItemBindingModel> model)
        {
            var invoice =
                await _invoiceService.CreateOrUpdateManualCharges(id, model.Select(_mapper.Map<InvoiceItem>).ToList());
            return Ok(_mapper.Map<InvoiceBindingModel>(invoice));
        }
    }
}
