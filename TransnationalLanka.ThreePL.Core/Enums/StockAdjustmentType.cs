using System.ComponentModel;

namespace TransnationalLanka.ThreePL.Core.Enums
{
    public enum StockAdjustmentType
    {
        [Description("In")]
        In = 1,
        [Description("Out")]
        Out = 2,
        [Description("ReturnIn")]
        ReturnIn = 3,
        [Description("ReturnOut")]
        ReturnOut = 4
    }

}
