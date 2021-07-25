using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;

namespace TransnationalLanka.ThreePl.Integration.Test
{
    [TestClass]
    public class TrackingApiServiceTest
    {
        [TestMethod]
        public async Task Test_Create_Customer()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.CreateCustomer(new CreateCustomerRequest()
            {
                CustomerName = "TRANSNATIONAL LANKA RECORD SOLUTIONS (PVT) LTD",
                CustomerAddress ="#55/60, VAUXHALL LANE, COLOMBO 02.",
                ContactNumber ="0117574574",
                EmailAddress ="vihanga.hettiarachchi@transnational-grp.com",
                NicNo = "000000000v",
                ContactPerson ="MR. VIHANGA HETTIARACHCHI",
                InvoicingHub ="0001",
                TaxRegNo = "100001100-2525",
                Vat = "1",
                Remarks ="Remarks",
                CreatedDate = System.DateTime.Now,
                TplSupplierCode = "1000005"
            });
            Assert.IsTrue(response.IsSuccess.Equals("1"));
        }
    }
}
