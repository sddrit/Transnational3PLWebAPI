using MediatR;

namespace TransnationalLanka.ThreePL.Services.Account.Request
{
    public class GetUserByIdRequestCommand : IRequest<Dal.Entities.User>
    {
        public long Id { get; set; }
    }
}
