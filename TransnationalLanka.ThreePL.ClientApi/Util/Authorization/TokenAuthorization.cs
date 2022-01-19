using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Api;
using TransnationalLanka.ThreePL.Services.Supplier;

namespace TransnationalLanka.ThreePL.ClientApi.Util.Authorization
{
    public class SupplierAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScemeName = "SupplierAuthenticationSchema";
        public string ClientIdHeaderName { get; set; } = "X-CLIENT-ID";
        public string ClientSecretHeaderName { get; set; } = "X-CLIENT-SECRET";
    }

    public class SupplierAuthHandler : AuthenticationHandler<SupplierAuthOptions>
    {
        private readonly IApiAccountService _apiAccountService;
        private readonly ISupplierService _supplierService;
        private readonly IAccountService _accountService;

        public SupplierAuthHandler(IOptionsMonitor<SupplierAuthOptions> options,
            IApiAccountService apiAccountService, IAccountService accountService,
            ISupplierService supplierService,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _apiAccountService = apiAccountService;
            _accountService = accountService;
            _supplierService = supplierService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.ClientIdHeaderName))
                return AuthenticateResult.Fail($"Missing Header {Options.ClientIdHeaderName}");

            if (!Request.Headers.ContainsKey(Options.ClientSecretHeaderName))
                return AuthenticateResult.Fail($"Missing Header {Options.ClientSecretHeaderName}");

            var clientId = Request.Headers[Options.ClientIdHeaderName];
            var clientSecret = Request.Headers[Options.ClientSecretHeaderName];

            try
            {
                var apiAccount = await _apiAccountService.GetAccountByClientIdAndSecret(clientId, clientSecret);
                var user = await _accountService.GetUserByIdSupplierId(apiAccount.SupplierId);
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                };
                var id = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(id);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (ServiceException e)
            {
                return AuthenticateResult.Fail(e);
            }
        }
    }
}
