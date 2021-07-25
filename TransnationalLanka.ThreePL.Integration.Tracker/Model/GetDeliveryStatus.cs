using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class GetDeliveryStatusRequest
    {
        [JsonPropertyName("TrackingNumber")]
        public string TrackingNumber { get; set; }
    }

    public class GetDeliveryStatusResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("result")]
        public GetDeliveryStatusResponseResult[] Result { get; set; }
    }

    public class GetDeliveryStatusResponseResult
    {
        [JsonPropertyName("hub")]
        public string Hub { get; set; }

        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonPropertyName("statusDate")]
        public DateTime StatusDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("statusUpdatedBy")]
        public string StatusUpdatedBy { get; set; }
    }
}
