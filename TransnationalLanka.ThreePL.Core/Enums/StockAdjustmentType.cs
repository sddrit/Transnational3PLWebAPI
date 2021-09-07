using System.ComponentModel;

namespace TransnationalLanka.ThreePL.Core.Enums
{
    public enum StockAdjustmentType
    {
        [Description("In")]
        In = 1,
        [Description("Out")]
        Out = 2,
        [Description("Return In")]
        DispatchReturnIn = 3,
        [Description("Return Out")]
        DispatchReturnOut = 4,
        [Description("Sales Return In")]
        SalesReturnIn = 5,
        [Description("Sales Return Out")]
        SalesReturnOut = 6,
        [Description("Damage In")]
        DamageIn = 7,
        [Description("Damage Out")]
        DamageOut = 8,
    }

}
