using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.WebApi.Models.Account;
using TransnationalLanka.ThreePL.WebApi.Util.Options;

namespace TransnationalLanka.ThreePL.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly TokenConfiguration _tokenConfiguration;

        public AccountController(IMapper mapper, IAccountService accountService, 
            IOptions<TokenConfiguration> tokenConfigurationOptions)
        {
            _mapper = mapper;
            _accountService = accountService;
            _tokenConfiguration = tokenConfigurationOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginBindingModel model)
        {
            var user = await _accountService.Login(model.UserName, model.Password);
            var roles = await _accountService.GetRoles(user);
            var token = await GenerateToken(user, roles.ToArray());

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var lifeTime = new JwtSecurityTokenHandler().ReadToken(jwtToken).ValidTo;

            return Ok(new LoginResponse
            {
                Token = jwtToken,
                ValidTo = lifeTime.ToUniversalTime(),
                User = _mapper.Map<UserBindingModel>(user),
                Roles = roles
            });
        }

        private async Task<JwtSecurityToken> GenerateToken(User user, string[] roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            AddRolesToClaims(claims, roles);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfiguration.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_tokenConfiguration.Issuer,
                _tokenConfiguration.Issuer,
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return token;
        }

        private ClaimsPrincipal ValidateToken(string accessToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfiguration.Key));

            var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, new TokenValidationParameters()
            {
                ValidAudience = _tokenConfiguration.Issuer,
                ValidIssuer = _tokenConfiguration.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            }, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }
    }
}
