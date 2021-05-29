using FluentValidation;
using TransnationalLanka.ThreePL.Services.Common.Validation;

namespace TransnationalLanka.ThreePL.Services.WareHouse.Core
{
    public class WareHouseValidator : AbstractValidator<Dal.Entities.WareHouse>
    {
        public WareHouseValidator()
        {
            RuleFor(p => p.WareHouseName).NotEmpty();
            RuleFor(p => p.Address).NotNull();
            RuleFor(p => p.Address).SetValidator(new WareHouseAddressValidator());
                      
        }
    }

    public class WareHouseAddressValidator : AbstractValidator<Dal.Entities.WareHouseAddress>
    {
        public WareHouseAddressValidator()
        {
            RuleFor(p => p.AddressLine1).NotEmpty();
            RuleFor(p => p.PostalCode).NotEmpty();
            RuleFor(p => p.CityId).NotEqual(0);
        }
    }
}
