﻿using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.WareHouse;
using TransnationalLanka.ThreePL.WebApi.Models.WareHouse;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWareHouseService _warehouseService;

        public WareHouseController(IMapper mapper, IWareHouseService warehouseService)
        {
            _mapper = mapper;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE, Roles.SUPPLIER_ROLE })]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(_warehouseService.GetWareHouses(), loadOptions);
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var warehouse = await _warehouseService.GetWareHouseById(id);
            return Ok(_mapper.Map<WareHouseBindingModel>(warehouse));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost("set-status")]
        public async Task<IActionResult> Post([FromBody]SetWareHouseStatus model)
        {
            await _warehouseService.SetWareHouseStatus(model.Id, model.Status);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]WareHouseBindingModel model)
        {
            var warehouse = await _warehouseService.AddWareHouse(_mapper.Map<WareHouse>(model));
            return Ok(_mapper.Map<WareHouseBindingModel>(warehouse));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] WareHouseBindingModel model)
        {
            var warehouse = await _warehouseService.UpdateWareHouse(_mapper.Map<WareHouse>(model));
            return Ok(_mapper.Map<WareHouseBindingModel>(warehouse));
        }
    }
}
