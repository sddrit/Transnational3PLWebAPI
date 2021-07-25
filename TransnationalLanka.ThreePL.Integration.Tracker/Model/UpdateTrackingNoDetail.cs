using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class UpdateTrackingNoDetailRequest
    {
        [JsonPropertyName("TrackingNo")]
        public string TrackingNo { get; set; }

        [JsonPropertyName("CODAmount")]
        public string CodAmount { get; set; }

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

        [JsonPropertyName("TPLWSBatchID")]
        public string TplWsBatchId { get; set; }

        [JsonPropertyName("InsertBy")]
        public string InsertBy { get; set; } = "PL_3PLWS";

        [JsonPropertyName("InsertedDate")]
        public DateTime InsertedDate { get; set; }
    }

    public class UpdateTrackingNoDetailResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("result")]
        public CreateCustomerResponseResult Result { get; set; }
    }
}
