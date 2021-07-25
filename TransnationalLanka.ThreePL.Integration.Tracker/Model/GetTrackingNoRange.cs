using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class GetTrackingNoRangeRequest
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("CustomerCode")]
        public string CustomerCode { get; set; }

    }

    public class GetTrackingNoRangeResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("result")]
        public GetTrackingNoRangeResponseResult Result { get; set; }

    }

    public class GetTrackingNoRangeResponseResult
    {
        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }
    }

}
