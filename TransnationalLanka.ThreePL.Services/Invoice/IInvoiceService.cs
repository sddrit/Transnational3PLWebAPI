﻿using System.Linq;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Invoice
{
    public interface IInvoiceService
    {
        IQueryable<Dal.Entities.Invoice> GetInvoices();
        Task<Dal.Entities.Invoice> GetInvoice(long id);
        Task<Dal.Entities.Invoice> MarkAsPaid(long id);
        Task GenerateInvoices();
    }
}