namespace TransnationalLanka.ThreePL.Services.WareHouse.Core
{
    public class WareHouseStorageInfo
    {
        public long WareHouseId { get; set; }
        public string Code { get; set; }
        public string WareHouseName { get; set; }
        public decimal StorageCapacity { get; set; }
        public decimal UsedSpace { get; set; }
        public decimal FreeSpace => StorageCapacity - UsedSpace;
    }
}
