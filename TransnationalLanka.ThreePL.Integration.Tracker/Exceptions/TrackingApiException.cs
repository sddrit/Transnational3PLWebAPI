using System;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Exceptions
{
    public class TrackingApiException : Exception
    {
        public TrackingApiException(string message): base(message)
        {
        }

        public TrackingApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
