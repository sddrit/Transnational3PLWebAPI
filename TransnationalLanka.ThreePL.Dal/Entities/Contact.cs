using Microsoft.EntityFrameworkCore;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
 
    [Owned]
    public class Contact
    {
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}