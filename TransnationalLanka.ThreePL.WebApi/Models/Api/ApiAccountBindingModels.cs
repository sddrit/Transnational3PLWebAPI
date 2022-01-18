namespace TransnationalLanka.ThreePL.WebApi.Models.Api
{
    public class ApiAccount
    {
        public long Id { get; set; }
        public bool Active { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }

    public class ApiAccountStatusBindingModel
    {
        public bool Status { get; set; }
    }
}
