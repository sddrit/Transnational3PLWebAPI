namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class UserWareHouse : BaseEntity
    {
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public long WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
    }
}
