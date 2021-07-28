using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class GetSetTrackingNumberDetailsRequest
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("CustomerCode")]
        public string CustomerCode { get; set; }

        [JsonPropertyName("TrackingNoCount")]
        public string TrackingNoCount { get; set; }

        [JsonPropertyName("CODAmount")]
        public string CODAmount { get; set; }

        [JsonPropertyName("ConsignorName")]
        public string ConsignorName { get; set; }

        [JsonPropertyName("ConsigneeName")]
        public string ConsigneeName { get; set; }

        [JsonPropertyName("ConsigneeAddress")]
        public string ConsigneeAddress { get; set; }

        [JsonPropertyName("ConsigneeCity")]
        public string ConsigneeCity { get; set; }

        [JsonPropertyName("ConsigneePhone")]
        public string ConsigneePhone { get; set; }

        [JsonPropertyName("InsertBy")]
        public string InsertBy { get; set; }

        [JsonPropertyName("InsertedDate")]
        public DateTime InsertedDate { get; set; }

        [JsonPropertyName("TPLWSBatchID")]
        public string TPLWSBatchID { get; set; }
    }

    public class GetSetTrackingNumberDetailsResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("result")]
        public GetSetTrackingNumberDetailsResponseResult Result { get; set; }

    }

    public class GetSetTrackingNumberDetailsResponseResult
    {
        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }
    }
}
