using FluentValidation;

namespace TransnationalLanka.ThreePL.Services.PurchaseOrder.Core
{
    public class PurchaseOrderValidator : AbstractValidator<Dal.Entities.PurchaseOrder>
    {
        public PurchaseOrderValidator()
        {
            RuleFor(p => p.SupplierId).NotEqual(0);
            RuleFor(p => p.PurchaseOrderItems).NotEmpty();

            RuleForEach(p => p.PurchaseOrderItems).SetValidator(new PurchaseOrderItemValidator());
        }
    }

    public class PurchaseOrderItemValidator : AbstractValidator<Dal.Entities.PurchaseOrderItem>
    {
        public PurchaseOrderItemValidator()
        {
            RuleFor(i => i.ProductId).NotEqual(0);
            RuleFor(i => i.Quantity).GreaterThan(0);
            RuleFor(i => i.UnitCost).GreaterThanOrEqualTo(0);
        }
    }
}
