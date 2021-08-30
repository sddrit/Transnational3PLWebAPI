using System.ComponentModel;

namespace TransnationalLanka.ThreePL.Core.Enums
{
    public enum DeliveryStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Processing")]
        Processing,
        [Description("Dispatched")]
        Dispatched,
        [Description("Partially Completed")]
        PartiallyCompleted,
        [Description("Completed")]
        Completed,
        [Description("Return")]
        Return,
        [Description("Partially Customer Return")]
        PartiallyCustomerReturn,
        [Description("Customer Return")]
        CustomerReturn
    }
}
