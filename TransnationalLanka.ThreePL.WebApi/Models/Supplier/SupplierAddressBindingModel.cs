namespace TransnationalLanka.ThreePL.WebApi.Models.Supplier
{
    public class SupplierAddressBindingModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long CityId { get; set; }
        public string PostalCode { get; set; }
    }
}