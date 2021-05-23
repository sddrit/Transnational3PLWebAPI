using AutoMapper;
using TransnationalLanka.ThreePL.Dal.Entities;
using TransnationalLanka.ThreePL.WebApi.Models.Account;

namespace TransnationalLanka.ThreePL.WebApi.Util.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            //Account Section
            CreateMap<User, UserBindingModel>();
        }
    }
}
