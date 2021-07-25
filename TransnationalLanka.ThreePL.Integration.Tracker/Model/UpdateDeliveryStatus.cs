using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class UpdateDeliveryStatusRequest
    {
        [JsonPropertyName("Hub")]
        public string Hub { get; set; }

        [JsonPropertyName("TrackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonPropertyName("StatusDate")]
        public DateTime StatusDate { get; set; }

        [JsonPropertyName("Status")]
        public string Status { get; set; }

        [JsonPropertyName("Reason")]
        public string Reason { get; set; }

        [JsonPropertyName("StatusUpdatedBy")]
        public string StatusUpdatedBy { get; set; } = "PL_3PLWS";

        [JsonPropertyName("StatusUpdatedDate")]
        public DateTime StatusUpdatedDate { get; set; }
    }
    public class UpdateDeliveryStatusResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("result")]
        public CreateCustomerResponseResult Result { get; set; }
    }
}
