using System;

namespace TransnationalLanka.ThreePL.Core.Environment
{
    public class CurrentEnvironment
    {
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
    }

}