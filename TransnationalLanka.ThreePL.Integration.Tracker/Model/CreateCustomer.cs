using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace TransnationalLanka.ThreePL.Integration.Tracker.Model
{
    public class CreateCustomerRequest
    {
        [JsonPropertyName("CustomerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("CustomerAddress")]
        public string CustomerAddress { get; set; }

        [JsonPropertyName("ContactNumber")]
        public string ContactNumber { get; set; }

        [JsonPropertyName("EmailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("NICNo")]
        public string NicNo { get; set; }

        [JsonPropertyName("ContactPerson")]
        public string ContactPerson { get; set; }

        [JsonPropertyName("InvoicingHub")] 
        public string InvoicingHub { get; set; } = "0001";

        [JsonPropertyName("TAXRegNo")]
        public string TaxRegNo { get; set; }

        [JsonPropertyName("VAT")]
        public string Vat { get; set; }

        [JsonPropertyName("Remarks")]
        public string Remarks { get; set; }

        [JsonPropertyName("Active")]
        public string Active { get; set; } = "1";

        [JsonPropertyName("Corporate")]
        public string Corporate { get; set; } = "1";

        [JsonPropertyName("CreatedBy")]
        public string CreatedBy { get; set; } = "PL_3PLWS";

        [JsonPropertyName("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("IsTPLSupplier")]
        public string IsTplSupplier { get; set; } = "1";

        [JsonPropertyName("TPLSupplierCode")]
        public string TplSupplierCode { get; set; }

    }

    public class CreateCustomerResponseResult
    {
        [JsonPropertyName("customerCode")]
        public string CustomerCode { get; set; }
    }

    public class CreateCustomerResponse
    {
        [JsonPropertyName("isSuccess")]
        public string IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("result")]
        public CreateCustomerResponseResult Result { get; set; }
    }


}
