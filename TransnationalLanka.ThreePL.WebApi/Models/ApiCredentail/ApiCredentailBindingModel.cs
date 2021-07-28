using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransnationalLanka.ThreePL.WebApi.Models.Common;

namespace TransnationalLanka.ThreePL.WebApi.Models.ApiCredentail
{
    public class ApiCredentailBindingModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Token { get; set; }

    }

}
