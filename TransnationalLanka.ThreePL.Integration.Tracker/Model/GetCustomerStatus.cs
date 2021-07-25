using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class GetCustomerStatusRequest
    {
        [JsonPropertyName("CustomerCode")]
        public string CustomerCode { get; set; }

    }

    public class GetCustomerStatusResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("result")]
        public GetCustomerStatusResponseResult[] Result { get; set; }

    }

    public class GetCustomerStatusResponseResult
    {
        [JsonPropertyName("customerCode")]
        public string CustomerCode { get; set; }

        [JsonPropertyName("customerStatus")]
        public string CustomerStatus { get; set; }

        [JsonPropertyName("statusDate")]
        public DateTime StatusDate { get; set; }

        [JsonPropertyName("statusSetBy")]
        public string StatusSetBy { get; set; }

        [JsonPropertyName("statusReason")]
        public string StatusReason { get; set; }
    }

}
