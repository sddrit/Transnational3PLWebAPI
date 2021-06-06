using FluentValidation;
using TransnationalLanka.ThreePL.Services.Common.Validation;

namespace TransnationalLanka.ThreePL.Services.Product.Core
{
    public class ProductValidator : AbstractValidator<Dal.Entities.Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.SupplierId).NotEqual(0);
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Code).NotEmpty();
            RuleFor(p => p.Description).NotEmpty();
            RuleFor(p => p.UnitPrice).NotEmpty();
            RuleFor(p => p.ReorderLevel).NotEmpty();
            RuleFor(p => p.Sku).NotEmpty();
            RuleFor(p => p.StorageUnits).NotEmpty();
        }
    }
   

}
