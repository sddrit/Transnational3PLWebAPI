using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            user.Active = true;

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

        public async Task<User> UpdateUser(User user, string role)
        {
            var currentUser = await GetUserById(user.Id);

            if (user == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"Unable to find user"
                    }
                });
            }

            currentUser.UserName = user.UserName;
            currentUser.Email = user.Email;
            currentUser.Active = user.Active;

            var addedWareHouses = user.UserWareHouses.Where(uw =>
                currentUser.UserWareHouses.All(cuw => cuw.WareHouseId != uw.WareHouseId)).ToList();

            var removedWareHouses = currentUser.UserWareHouses
                .Where(cuw => user.UserWareHouses.All(uw => uw.WareHouseId != cuw.WareHouseId)).ToList();

            foreach (var removedWareHouse in removedWareHouses)
            {
                currentUser.UserWareHouses.Remove(removedWareHouse);
            }

            foreach (var addedWareHouse in addedWareHouses)
            {
                currentUser.UserWareHouses.Add(new UserWareHouse()
                {
                    UserId = currentUser.Id,
                    WareHouseId = addedWareHouse.WareHouseId
                });
            }

            var updatedResult = await _userManager.UpdateAsync(user);

            if (!updatedResult.Succeeded)
            {
                throw GenerateServiceException(updatedResult);
            }

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);
            }

            return currentUser;

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

            if (!user.Active)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = "User is not an active user"
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

        public async Task<User> SetStatus(long id, bool status)
        {
            var user = await GetUserById(id);

            if (user == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"User is not found"
                    }
                });
            }

            user.Active = status;
            await _userManager.UpdateAsync(user);
            return user;
        }

        public async Task DeleteUser(long id)
        {
            var user = await GetUserById(id);

            if (user == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"User is not found"
                    }
                });
            }

            await _userManager.DeleteAsync(user);
        }

        public IQueryable<User> GetUsers()
        {
            return _userManager.Users;
        }

        public async Task ResetPassword(long id, string newPassword)
        {
            var user = await GetUserById(id);

            if (user == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"User is not found"
                    }
                });
            }

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);

            if (!removePasswordResult.Succeeded)
            {
                throw GenerateServiceException(removePasswordResult);
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

            if (!addPasswordResult.Succeeded)
            {
                throw GenerateServiceException(addPasswordResult);
            }
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
            var user = await _userManager.FindByNameAsync(username);
            return await GetUserById(user.Id);
        }

        public async Task<User> GetUserById(long id)
        {
            return await _userManager.Users
                .Include(u => u.UserWareHouses)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUser(ClaimsPrincipal user)
        {
            var userId = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            return await GetUserById(userId);
        }

        public async Task<IList<string>> GetRoles()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToArrayAsync();
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
