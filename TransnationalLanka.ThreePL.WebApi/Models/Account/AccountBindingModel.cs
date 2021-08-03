using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransnationalLanka.ThreePL.WebApi.Models.Account
{
    public class LoginBindingModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserBindingModel
    {
        public long Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool Active { get; set; }
    }

    public class CreateUserBindingModel
    {
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

    public class LoginResponse
    {
        public string Token { get; set; }
        public UserBindingModel User { get; set; }
        public DateTimeOffset ValidTo { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class SetUserStatus
    {
        public long Id { get; set; }
        public bool Status { get; set; }
    }
}
