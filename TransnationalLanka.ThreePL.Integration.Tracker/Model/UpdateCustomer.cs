using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class UpdateCustomerRequest
    {
        [JsonPropertyName("CustomerCode")]
        public string CustomerCode { get; set; }

        [JsonPropertyName("StatusDate")]
        public DateTime StatusDate { get; set; }

        [JsonPropertyName("StatusSetBy")]
        public string StatusSetBy { get; set; } = "PL_3PLWS";

        [JsonPropertyName("StatusReason")]
        public string StatusReason { get; set; }

        [JsonPropertyName("Active")]
        public string Active { get; set; }

    }
    public class UpdateCustomerResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("result")]
        public CreateCustomerResponseResult Result { get; set; }
    }
}
