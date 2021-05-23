using System.Collections.Generic;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Account
{
    public interface IAccountService
    {
        Task<User> CreateUser(User user, string password, string role);
        Task<User> Login(string username, string password);
        Task CreateRole(string roleName);
        Task<User> GetUserByUserName(string username);
        Task<User> GetUserById(long id);
        Task<IList<string>> GetRoles(User user);
    }
}