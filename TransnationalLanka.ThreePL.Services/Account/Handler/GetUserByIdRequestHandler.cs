using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.Services.Account.Request;

namespace TransnationalLanka.ThreePL.Services.Account.Handler
{
    public class GetUserByIdRequestHandler : IRequestHandler<GetUserByIdRequestCommand, Dal.Entities.User>
    {
        private readonly IAccountService _accountService;

        public GetUserByIdRequestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<User> Handle(GetUserByIdRequestCommand request, CancellationToken cancellationToken)
        {
            return await _accountService.GetUserById(request.Id);
        }
    }
}
