using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Application;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet("get-audit-logs")]
        public async Task<LoadResult> GetAuditLogs(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(_logService.GetAuditLogs(), loadOptions);
        }

        [HttpGet("get-events")]
        public async Task<LoadResult> GetEvents(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(_logService.GetEvents(), loadOptions);
        }
    }
}
