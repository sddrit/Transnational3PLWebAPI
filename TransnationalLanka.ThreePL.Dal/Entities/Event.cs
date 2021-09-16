using System;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Event
    {
        public long Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public string EventType { get; set; }
        public string JsonData { get; set; }
        public string User { get; set; }
    }
}
