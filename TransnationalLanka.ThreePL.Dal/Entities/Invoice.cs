using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TransnationalLanka.ThreePL.Core.Enums;

namespace TransnationalLanka.ThreePL.Dal.Entities
{
    public class Invoice : BaseEntity
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TaxType TaxType { get; set; }
        public string InvoiceNo { get; set; }
        public long SupplierId { get; set; }
        public bool Paid { get; set; }
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
        public decimal Total
        {
            get
            {
                if (TaxType == TaxType.SVat)
                {
                    return SubTotal;
                }

                return SubTotal + Tax;
            }
        }
    }

    public enum InvoiceItemChargeType
    {
        StorageCharge,
        StorageAdditionalCharge,
        PackageCharge,
        ManualCharges
    }

    public class InvoiceItem : BaseEntity
    {
        public DateTime? Date { get; set; }
        public InvoiceItemChargeType Type { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        [NotMapped]
        public decimal Amount => Rate * Quantity;
    }
}
