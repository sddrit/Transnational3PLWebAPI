using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Account
{
    public interface IAccountService
    {
        Task<User> CreateUser(User user, string password, string role);
        Task<User> UpdateUser(User user);
        Task<User> SetStatus(long id, bool status);
        Task<User> Login(string username, string password);
        Task CreateRole(string roleName);
        Task<User> GetUserByUserName(string username);
        Task<User> GetUserById(long id);
        Task<IList<string>> GetRoles(User user);
        Task ResetPassword(string username, string newPassword);
        IQueryable<User> GetUsers();
        Task<User> GetUser(ClaimsPrincipal user);
    }
}