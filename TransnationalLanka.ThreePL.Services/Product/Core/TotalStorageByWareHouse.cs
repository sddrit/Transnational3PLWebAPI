namespace TransnationalLanka.ThreePL.Services.Product.Core
{
    public class TotalStorageByWareHouse
    {
        public long WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public string WareHouseCode { get; set; }
        public decimal TotalStorage { get; set; }
    }
}
