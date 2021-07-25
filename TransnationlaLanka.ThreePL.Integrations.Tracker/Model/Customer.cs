using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalaLanka.ThreePLlaLanka.ThreePL.Integrations.Tracker.Model
{
    public class Customer
    {

        public class Root
        {
            [JsonProperty("CustomerName")]
            public string CustomerName { get; set; }

            [JsonProperty("CustomerAddress")]
            public string CustomerAddress { get; set; }

            [JsonProperty("ContactNumber")]
            public string ContactNumber { get; set; }

            [JsonProperty("EmailAddress")]
            public string EmailAddress { get; set; }

            [JsonProperty("NICNo")]
            public string NICNo { get; set; }

            [JsonProperty("ContactPerson")]
            public string ContactPerson { get; set; }

            [JsonProperty("InvoicingHub")]
            public string InvoicingHub { get; set; }

            [JsonProperty("TAXRegNo")]
            public string TAXRegNo { get; set; }

            [JsonProperty("VAT")]
            public string VAT { get; set; }

            [JsonProperty("Remarks")]
            public string Remarks { get; set; }

            [JsonProperty("Active")]
            public string Active { get; set; }

            [JsonProperty("Corporate")]
            public string Corporate { get; set; }

            [JsonProperty("CreatedBy")]
            public string CreatedBy { get; set; }

            [JsonProperty("CreatedDate")]
            public string CreatedDate { get; set; }

            [JsonProperty("IsTPLSupplier")]
            public string IsTPLSupplier { get; set; }

            [JsonProperty("TPLSupplierCode")]
            public string TPLSupplierCode { get; set; }
        }


    }
}
