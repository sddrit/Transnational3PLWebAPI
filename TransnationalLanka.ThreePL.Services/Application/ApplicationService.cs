using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account;
using TransnationalLanka.ThreePL.Services.Account.Core;
using TransnationalLanka.ThreePL.Services.Setting;

namespace TransnationalLanka.ThreePL.Services.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly IAccountService _accountService;
        private readonly ISettingService _settingService;
        private readonly ThreePlDbContext _context;

        public ApplicationService(ThreePlDbContext context, IAccountService accountService, ISettingService settingService)
        {
            _context = context;
            _accountService = accountService;
            _settingService = settingService;
        }

        public async Task Initial()
        {
            await _context.Database.MigrateAsync();

            //Creating the roles
            await _accountService.CreateRole(Roles.ADMIN_ROLE);
            await _accountService.CreateRole(Roles.SUPPLIER_ROLE);
            await _accountService.CreateRole(Roles.USER_ROLE);
            await _accountService.CreateRole(Roles.WAREHOUSE_MANAGER_ROLE);

            //Creating the admin
            if (await _accountService.GetUserByUserName("admin") == null)
            {
                await _accountService.CreateUser(new User() {UserName = "admin", Email = "slakjaya@gmail.com"}, "1234Qwer@", Roles.ADMIN_ROLE);
            }

            //Create tax percentage settings
            await _settingService.CreateOrUpdateValue(Settings.TAX_PERCENTAGE, "0.08");
        }
    }
}
