using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Product;
using TransnationalLanka.ThreePL.Services.Supplier;

namespace TransnationalLanka.ThreePL.Services.Invoice
{
    public class InvoiceService
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
    }
}
