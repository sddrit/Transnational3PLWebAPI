using Microsoft.EntityFrameworkCore;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    [Owned]
    public class SupplierCharges
    {
        //Pre Allocated Storage Units
        public decimal AllocatedUnits { get; set; }
        //Storage charge per unit
        public decimal StorageChargePerUnit { get; set; }
        //Additional Charges
        public decimal AdditionalChargePerUnitPrice { get; set; }
        //Packaging Charge Per Item
        public decimal HandlingCharge { get; set; }

        public decimal CalculateStorageCharge(int units)
        {
            if (AllocatedUnits <= units)
            {
                return AllocatedUnits * StorageChargePerUnit;
            }

            return AllocatedUnits * StorageChargePerUnit + (units - AllocatedUnits) * AdditionalChargePerUnitPrice;
        }
    }
}
