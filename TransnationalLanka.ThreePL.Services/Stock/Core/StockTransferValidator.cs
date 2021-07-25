using FluentValidation;

namespace TransnationalLanka.ThreePL.Services.Stock.Core
{
    public class StockTransferValidator : AbstractValidator<Dal.Entities.StockTransfer>
    {
        public StockTransferValidator()
        {
            RuleFor(s => s.FromWareHouseId).NotEqual(0);
            RuleFor(s => s.ToWareHouseId).NotEqual(0);

            RuleFor(s => s.StockTransferItems).NotEmpty();
            RuleForEach(s => s.StockTransferItems).SetValidator(new StockTransferItemValidator());
        }
    }

    public class StockTransferItemValidator : AbstractValidator<Dal.Entities.StockTransferItem>
    {
        public StockTransferItemValidator()
        {
            RuleFor(i => i.ProductId).NotEqual(0);
            RuleFor(i => i.Quantity).GreaterThan(0);
            RuleFor(i => i.UnitCost).GreaterThanOrEqualTo(0);
        }
    }
}
