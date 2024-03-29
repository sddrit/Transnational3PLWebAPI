﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Audit.WebApi;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Util;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [AuditApi(IncludeRequestBody = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(_cityService.GetCities(), loadOptions);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            return Ok(await _cityService.GetCityById(id));
        }
    }
}
