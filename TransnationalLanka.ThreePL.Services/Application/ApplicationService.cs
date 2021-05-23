using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;

namespace TransnationalLanka.ThreePL.Services.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly IAccountService _accountService;

        public ApplicationService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task Initial()
        {
            //Creating the roles
            await _accountService.CreateRole(Roles.ADMIN_ROLE);
            await _accountService.CreateRole(Roles.SUPPLIER_ROLE);

            //Creating the admin
            if (await _accountService.GetUserByUserName("admin") == null)
            {
                await _accountService.CreateUser(new User() {UserName = "admin", Email = "slakjaya@gmail.com"}, "1234Qwer@", Roles.ADMIN_ROLE);
            }
        }
    }
}
