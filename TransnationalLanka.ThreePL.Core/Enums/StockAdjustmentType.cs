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
        ReturnIn = 3,
        [Description("Return Out")]
        ReturnOut = 4
    }

}
