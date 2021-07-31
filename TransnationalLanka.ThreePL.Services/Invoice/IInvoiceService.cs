using System.Linq;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Invoice
{
    public interface IInvoiceService
    {

        Task GenerateInvoice(long supplierId);
        Task<Dal.Entities.Invoice> GetInvoiceById(long id);


    }
}