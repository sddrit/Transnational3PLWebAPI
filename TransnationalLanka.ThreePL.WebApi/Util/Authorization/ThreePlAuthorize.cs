using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TransnationalLanka.ThreePL.WebApi.Util.Authorization
{
    public class ThreePlAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public ThreePlAuthorize(string[] roles = null)
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ContentType = "application/json",
                    Content = System.Text.Json.JsonSerializer.Serialize(new {error = "Unauthorized"})
                };
                return;
            }

            if (_roles != null && _roles.All(role => !user.IsInRole(role)))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ContentType = "application/json",
                    Content = System.Text.Json.JsonSerializer.Serialize(new {error = "Unauthorized"})
                };
                return;
            }
        }
    }
}
