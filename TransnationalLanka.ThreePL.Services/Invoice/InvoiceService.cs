using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Supplier;

namespace TransnationalLanka.ThreePL.Services.Invoice
{
    public class InvoiceService:IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;

        public InvoiceService(IUnitOfWork unitOfWork, ISupplierService supplierService, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _supplierService = supplierService;
            _productService = productService;
        }

        private async Task GenerateInvoice(long supplierId)
        {
            var supplier = await _supplierService.GetSupplierById(supplierId);

            if (!_supplierService.IsActiveSupplier(supplier))
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = $"{supplier.SupplierName} is not a active one"
                    }
                });
            }
        }

        public async Task<Dal.Entities.Invoice> GetInvoiceById(long id)
        {
            var invoice = await _unitOfWork.InvoiceRepository.GetAll()
                .Include(i => i.InvoiceItems)               
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"Unable to find invoice by id {id}"
                    }
                });
            }

            return invoice;
        }

        Task IInvoiceService.GenerateInvoice(long supplierId)
        {
            throw new System.NotImplementedException();
        }
    }
}
