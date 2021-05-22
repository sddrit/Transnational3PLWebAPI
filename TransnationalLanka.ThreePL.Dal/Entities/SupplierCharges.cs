using Microsoft.EntityFrameworkCore;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    [Owned]
    public class SupplierCharges
    {
        //Pre Allocated Charge Details
        public int AllocatedUnits { get; set; }
        //This is a fixed price for calculate the storage price
        public decimal AllocatedUnitsFixedPrice { get; set; }
        //Additional Charges
        public decimal AdditionalChargePerUnitPrice { get; set; }
        //Packaging Charge Per Item
        public decimal HandlingCharge { get; set; }

        public decimal CalculateStorageCharge(int units)
        {
            if (AllocatedUnits <= units)
            {
                return AllocatedUnitsFixedPrice;
            }

            return AllocatedUnitsFixedPrice + (units - AllocatedUnits) * AllocatedUnitsFixedPrice;
        }
    }
}
