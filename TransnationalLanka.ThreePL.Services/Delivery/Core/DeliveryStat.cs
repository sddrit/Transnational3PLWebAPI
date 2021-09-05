using System;
using System.Collections.Generic;
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.Services.Delivery.Core
{
    public class DeliveryStat
    {
        public DeliveryStatus Status { get; set; }
        public long Count { get; set; }
    }

    public class DayDeliveryStat
    {
        public DateTime Date { get; set; }
        public List<DeliveryStat> DeliveryStats { get; set; }
    }
}
