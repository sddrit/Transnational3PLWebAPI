using System.ComponentModel.DataAnnotations;

namespace TransnationalLanka.ThreePL.WebApi.Models.Supplier
{
    public class CreateSupplierAccount
    {
        public long SupplierId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmationPassword { get; set; }
    }
}
