using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Invoice : BaseEntity
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string InvoiceNo { get; set; }
        public long SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
        
        [NotMapped]
        public decimal SubTotal {
            get
            {
                if (InvoiceItems != null && InvoiceItems.Any())
                {
                    return InvoiceItems.Sum(i => i.Amount);
                }

                return 0;
            }
        }

        public decimal TaxPercentage { get; set; }
        [NotMapped]
        public decimal Tax => SubTotal * TaxPercentage;
        [NotMapped] 
        public decimal Total => SubTotal + Tax;
    }

    public enum InvoiceItemChargeType
    {
        StorageCharge,
        StorageAdditionalCharge,
        PackageCharge,

    }

    public class InvoiceItem : BaseEntity
    {
        public InvoiceItemChargeType Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
