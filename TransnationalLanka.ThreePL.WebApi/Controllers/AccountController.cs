using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.WebApi.Models.Account;
using TransnationalLanka.ThreePL.WebApi.Util.Authorization;
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

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpGet]
        public async Task<LoadResult> Get(DataSourceLoadOptions loadOptions)
        {
            var query = _mapper.ProjectTo<UserBindingModel>(_accountService.GetUsers());
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var user = await _accountService.GetUserById(id);
            return Ok(await GetUserResponse(user));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserBindingModel model)
        {
            var user = new User() { UserName = model.UserName, Email = model.Email, Active = true };

            if (model.WareHouses != null && model.WareHouses.Any())
            {
                user.UserWareHouses = model.WareHouses.Select(wareHouse => new UserWareHouse()
                {
                    WareHouseId = wareHouse
                }).ToList();
            }

            var createdUser = await _accountService.CreateUser(user, model.Password, model.Role);
            return Ok(await GetUserResponse(createdUser));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UserBindingModel model)
        {
            var user = await _accountService.GetUserById(model.Id);

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Active = model.Active;

            if (model.WareHouses != null && model.WareHouses.Any())
            {
                user.UserWareHouses = model.WareHouses.Select(wareHouse => new UserWareHouse()
                {
                    WareHouseId = wareHouse
                }).ToList();
            }
            else
            {
                user.UserWareHouses = new List<UserWareHouse>();
            }

            var updatedUser = await _accountService.UpdateUser(user, model.Role);

            return Ok(await GetUserResponse(updatedUser));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost("set-status")]
        public async Task<IActionResult> SetStatus([FromBody] SetUserStatus model)
        {
            var updatedUser = await _accountService.SetStatus(model.Id, model.Status);
            return Ok(await GetUserResponse(updatedUser));
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordBindingModel model)
        {
            await _accountService.ResetPassword(model.Id, model.Password);
            return Ok();
        }

        [ThreePlAuthorize(new[] { Roles.ADMIN_ROLE })]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _accountService.DeleteUser(id);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
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
                User = await GetUserResponse(user),
            });
        }

        private async Task<UserBindingModel> GetUserResponse(User user)
        {
            var userBindingModel = _mapper.Map<UserBindingModel>(user);
            var roles = await _accountService.GetRoles(user);

            userBindingModel.Role = roles.FirstOrDefault();
            return userBindingModel;
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
