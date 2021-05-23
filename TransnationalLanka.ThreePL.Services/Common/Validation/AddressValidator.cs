using FluentValidation;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Common.Validation
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(p => p.AddressLine1).NotEmpty();
            RuleFor(p => p.PostalCode).NotEmpty();
            RuleFor(p => p.CityId).NotEqual(0);
        }
    }
}
