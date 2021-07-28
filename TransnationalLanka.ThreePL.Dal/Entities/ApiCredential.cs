using System.Collections.Generic;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class ApiCredential : BaseEntity
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }

    }
}
