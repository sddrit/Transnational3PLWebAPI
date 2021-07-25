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

        [TestMethod]
        public async Task Test_Update_Customer()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.UpdateCustomerStatus(new UpdateCustomerRequest()
            {
                CustomerCode="T001",
	            StatusDate= System.DateTime.Now,	           
	            StatusReason ="PAYMENT ISSUES",
	            Active ="0"
            });
            Assert.IsTrue(response.IsSuccess.Equals("1"));
        }

        [TestMethod]
        public async Task Test_Delivery_Status()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.UpdateDeliveryStatus(new UpdateDeliveryStatusRequest()
            {
                Hub="0051",
	            TrackingNumber="19940801",
	            StatusDate= System.DateTime.Now,
	            Status="Reject",
	            Reason="REFUSED BY THE RECIPIENT",	         
	            StatusUpdatedDate= System.DateTime.Now
            });
            Assert.IsTrue(response.IsSuccess.Equals("1"));
        }

        [TestMethod]
        public async Task Test_UpdateTrackingNo_Details()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.UpdateTrackingNoDetails(new UpdateTrackingNoDetailRequest()
            {
                TrackingNo= "COD2649008",
	            CodAmount= "1999.00",
	            ConsignorName= "T001",
	            ConsigneeName="SDDR",
	            ConsigneeAddress= "NO. 55/60, VAUXHALL LANE,",
	            ConsigneeCity= "COLOMBO - 02.",
	            ConsigneePhone= "0117574574",
	            TplWsBatchId= "000001",	           
	            InsertedDate= System.DateTime.Now
            });
            Assert.IsTrue(response.IsSuccess.Equals("1"));
        }

        [TestMethod]
        public async Task Test_Get_Customer_Status()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.GetCustomerStatus(new GetCustomerStatusRequest()
            {
               CustomerCode= "T001"
            });
            Assert.IsTrue(!response.Result.Equals(null));
        }

        [TestMethod]
        public async Task Test_Get_Delivery_Status()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.GetDeliveryStatus(new GetDeliveryStatusRequest()
            {
                TrackingNumber = "19940801"
            });
            Assert.IsTrue(!response.Result.Equals(null));
        }


        [TestMethod]
        public async Task Test_Get_TrackingNo_Range()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.GetTrackingNoRange(new GetTrackingNoRangeRequest()
            {
                Type = "1",
                CustomerCode= "T001"
            });
            Assert.IsTrue(!response.Result.Equals(null));
        }

        [TestMethod]
        public async Task Test_Get_Inward()
        {
            var trackingApiService = new TrackerApiService(true);
            var response = await trackingApiService.GetInward(new GetInwardRequest()
            {
                TrackingNumber= "19940801"
            });
            Assert.IsTrue(!response.Result.Equals(null));
        }
    }

}
