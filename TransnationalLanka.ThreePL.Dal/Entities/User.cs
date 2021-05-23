﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class User : IdentityUser<long>
    {
        public long? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}