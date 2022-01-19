using System;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using TransnationalLanka.ThreePL.Core.Environment;
using TransnationalLanka.ThreePL.Services.Account.Request;

namespace TransnationalLanka.ThreePL.ClientApi.Util.Enviroment
{
    public class ClientApiEnviroment : IEnvironment
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public ClientApiEnviroment(IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        public CurrentEnvironment GetCurrentEnvironment()
        {
            if (_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return new CurrentEnvironment()
                {
                    Service = "Client API",
                    MachineName = Environment.MachineName,
                    UserId = null,
                    UserName = Environment.MachineName
                };
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var username = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = _mediator.Send(new GetUserByIdRequestCommand() { Id = long.Parse(userId) }).Result;
                username = user.UserName;
            }

            return new CurrentEnvironment()
            {
                Service = "Client API",
                MachineName = Environment.MachineName,
                UserId = string.IsNullOrEmpty(userId) ? null : long.Parse(userId),
                UserName = username
            };
        }
    }
}
