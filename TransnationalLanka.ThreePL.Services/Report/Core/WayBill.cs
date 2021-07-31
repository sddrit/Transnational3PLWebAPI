using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Report.Core
{
    public class WayBill
    {
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }      
        public string WareHouseAddress{ get; set; }
       
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string DeliveryNumber { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddress { get; set; }
       
        public string[] TrackingNo { get; set; }
        public decimal DeliveryPrice { get; set; }
        public List<WayBillItem> WayBillItems { get; set; }

    }
    public class WayBillItem
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public string UnitOfMeasure { get; set; }
        public decimal? UnitWeight { get; set; }

    }
    
}
