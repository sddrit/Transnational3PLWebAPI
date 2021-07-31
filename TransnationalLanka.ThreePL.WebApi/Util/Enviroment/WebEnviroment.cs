using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TransnationalLanka.ThreePL.Core.Environment;
using TransnationalLanka.ThreePL.Services.Account;

namespace TransnationalLanka.ThreePL.WebApi.Util.Enviroment
{
    public class WebEnvirnoment : IEnvironment
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        public WebEnvirnoment(IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
        }
        public CurrentEnvironment GetCurrentEnvironment()
        {
            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return new CurrentEnvironment()
                {
                    Service = "Web API",
                    MachineName = Environment.MachineName,
                    UserId = null,
                    UserName = "Admin"
                };
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var username = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = _accountService.GetUserById(long.Parse(userId)).Result;
                username = user.UserName;
            }

            return new CurrentEnvironment()
            {
                Service = "Web API",
                MachineName = Environment.MachineName,
                UserId = string.IsNullOrEmpty(userId)? null: long.Parse(userId),
                UserName = username
            };
        }
    }
}
