using System.ComponentModel;

namespace TransnationalLanka.ThreePL.Core.Enums
{
    public enum TrackingStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Dispatched")]
        Dispatched,
        [Description("Completed")]
        Completed,
        [Description("Returned")]
        Returned
    }
}
