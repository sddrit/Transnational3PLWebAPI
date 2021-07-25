using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class GetInwardRequest
    {
        [JsonPropertyName("TrackingNumber")]
        public string TrackingNumber { get; set; }
    }

    public class GetInwardResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("result")]
        public GetInwardResponseResult[] Result { get; set; }

    }

    public class GetInwardResponseResult
    {
        [JsonPropertyName("trackingNo")]
        public string TrackingNo { get; set; }

        [JsonPropertyName("inwardedHub")]
        public string InwardedHub { get; set; }

        [JsonPropertyName("customerCode")]
        public string CustomerCode { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("customerAddress")]
        public string CustomerAddress { get; set; }

        [JsonPropertyName("customerNumber")]
        public string CustomerNumber { get; set; }

        [JsonPropertyName("receiverName")]
        public string ReceiverName { get; set; }

        [JsonPropertyName("receiverAddress")]
        public string ReceiverAddress { get; set; }

        [JsonPropertyName("receiverNumber")]
        public string ReceiverNumber { get; set; }

        [JsonPropertyName("tplwsBatchID")]
        public string TplwsBatchID { get; set; }

        [JsonPropertyName("snicNo")]
        public string SnicNo { get; set; }

        [JsonPropertyName("rnicNo")]
        public string RnicNo { get; set; }

        [JsonPropertyName("toLocation")]
        public string ToLocation { get; set; }

        [JsonPropertyName("area")]
        public string Area { get; set; }

        [JsonPropertyName("itemType")]
        public string ItemType { get; set; }

        [JsonPropertyName("itemWeight")]
        public string ItemWeight { get; set; }

        [JsonPropertyName("taxRegNo")]
        public string TaxRegNo { get; set; }

        [JsonPropertyName("inwardedBy")]
        public string InwardedBy { get; set; }

        [JsonPropertyName("inwardedDate")]
        public string InwardedDate { get; set; }

        [JsonPropertyName("codAmount")]
        public string CodAmount { get; set; }
    }

}
