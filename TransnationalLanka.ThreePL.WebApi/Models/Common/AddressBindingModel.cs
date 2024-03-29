﻿using System.ComponentModel.DataAnnotations;

namespace TransnationalLanka.ThreePL.WebApi.Models.Common
{
    public class AddressBindingModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required]
        public long CityId { get; set; }
        public CityBindingModel City { get; set; }
        [Required]
        public string PostalCode { get; set; }
    }
}