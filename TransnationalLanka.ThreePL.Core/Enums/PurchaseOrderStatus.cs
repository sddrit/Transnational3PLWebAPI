using System.ComponentModel;

namespace TransnationalLanka.ThreePL.Core.Enums
{
    public enum PurchaseOrderStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Partially Received")]
        PartiallyReceived,
        [Description("Received")]
        Received
    }
}
