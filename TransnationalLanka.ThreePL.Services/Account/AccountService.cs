using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AccountService(IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> CreateUser(User user, string password, string role)
        {
            await using var transaction = await _unitOfWork.GetTransaction();

            var createdUserResult = await _userManager.CreateAsync(user, password);

            if (!createdUserResult.Succeeded)
            {
                await transaction.RollbackAsync();
                throw GenerateServiceException(createdUserResult);
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);

            if (!addToRoleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                throw GenerateServiceException(addToRoleResult);
            }

            await transaction.CommitAsync();

            return user;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await GetUserByUserName(username);

            if (user == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"{username} is not found"
                    }
                });
            }

            var isCorrectPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!isCorrectPassword)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = "Invalid password"
                    }
                });
            }

            return user;
        }

        public async Task<IList<string>> GetRoles(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task CreateRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            await _roleManager.CreateAsync(new Role()
            {
                Name = roleName
            });
        }

        public async Task<User> GetUserByUserName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<User> GetUserById(long id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        private ServiceException GenerateServiceException(IdentityResult identityResult)
        {
            var exceptionMessages = identityResult.Errors.Select(error => new ErrorMessage()
            {
                Code = error.Code,
                Message = error.Description
            });

            return new ServiceException(exceptionMessages.ToArray());
        }
    }
}
