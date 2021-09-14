using System;
using System.Collections.Generic;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.WebApi.Models.Invoice
{
    public class InvoiceBindingModel
    {
        public long Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string InvoiceNo { get; set; }
        public long SupplierId { get; set; }
        public bool Paid { get; set; }
        public virtual ICollection<InvoiceItemBindingModel> InvoiceItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }

    public class InvoiceItemBindingModel
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public InvoiceItemChargeType Type { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
    }
}
