using FluentValidation;
using TransnationalLanka.ThreePL.Services.Common.Validation;

namespace TransnationalLanka.ThreePL.Services.Supplier.Core
{
    public class SupplierValidator : AbstractValidator<Dal.Entities.Supplier>
    {
        public SupplierValidator()
        {
            RuleFor(p => p.SupplierName).NotEmpty();
            RuleFor(p => p.Address).NotNull();
            RuleFor(p => p.Address).SetValidator(new SupplierAddressValidator());

            RuleFor(p => p.Contact).NotNull();
            RuleFor(p => p.Contact.Phone).NotEmpty();

            RuleForEach(p => p.PickupAddress).SetValidator(new AddressValidator());
        }
    }

    public class SupplierAddressValidator : AbstractValidator<Dal.Entities.SupplierAddress>
    {
        public SupplierAddressValidator()
        {
            RuleFor(p => p.AddressLine1).NotEmpty();
            RuleFor(p => p.PostalCode).NotEmpty();
            RuleFor(p => p.CityId).NotEqual(0);
        }
    }
}
