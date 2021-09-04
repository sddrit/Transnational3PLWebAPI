namespace TransnationalLanka.ThreePL.Services.Delivery.Core
{
    public class ProcessDeliverCompleteResult
    {
        public string TrackingNumber { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
