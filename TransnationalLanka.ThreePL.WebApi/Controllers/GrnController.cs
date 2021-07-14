using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Grn;
using TransnationalLanka.ThreePL.WebApi.Models.Grn;
using DevExtreme.AspNet.Mvc;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrnController : ControllerBase
    {
        private readonly IGrnService _grnService;
        private readonly IMapper _mapper;

        public GrnController(IMapper mapper, IGrnService grnService)
        {
            _mapper = mapper;
            _grnService = grnService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<GoodReceivedNoteBindingModel>(_grnService.GetAll());
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
