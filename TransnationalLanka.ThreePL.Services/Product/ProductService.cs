using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Services.Common.Mapper;
using TransnationalLanka.ThreePL.Services.Product.Core;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Dal.Entities.Product> GetProducts()
        {
            return _unitOfWork.ProductRepository.GetAll()
                .Include(p => p.Supplier);
        }

        public async Task<Dal.Entities.Product> AddProduct(Dal.Entities.Product product)
        {
            Guard.Argument(product, nameof(product)).NotNull();

            await ValidateProduct(product);

            _unitOfWork.ProductRepository.Insert(product);
            await _unitOfWork.SaveChanges();

            return product;
        }

        public async Task<Dal.Entities.Product> UpdateProduct(Dal.Entities.Product product)
        {
            Guard.Argument(product, nameof(Product)).NotNull();

            await ValidateProduct(product);

            var currentProduct = await GetProductById(product.Id);

            var mapper = ServiceMapper.GetMapper();
            mapper.Map(product, currentProduct);

            await _unitOfWork.SaveChanges();

            return currentProduct;
        }

        public async Task<Dal.Entities.Product> GetProductById(long id)
        {
            var product = await _unitOfWork.ProductRepository.GetAll()
                .Include(p => p.Supplier)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Code = string.Empty,
                        Message = $"Unable to find Product by id {id}"
                    }
                });
            }
            return product;
        }

        public async Task SetProductStatus(long id, bool status)
        {
            var currentProduct = await GetProductById(id);
            currentProduct.Active = status;
            await _unitOfWork.SaveChanges();

            //Todo send the status change to tracking application
        }

        public bool IsActiveProduct(Dal.Entities.Product product)
        {
            return product.Active;
        }

        private async Task ValidateProduct(Dal.Entities.Product product)
        {
            var productValidator = new ProductValidator();
            var validateResult = await productValidator.ValidateAsync(product);

            if (validateResult.IsValid)
            {
                return;
            }

            throw new ServiceException(validateResult.Errors.Select(e => new ErrorMessage()
            {
                Code = ErrorCodes.Model_Validation_Error_Code,
                Meta = new
                {
                    e.ErrorCode,
                    e.ErrorMessage,
                    e.PropertyName
                },
                Message = e.ErrorMessage
            }).ToArray());
        }
    }
}
