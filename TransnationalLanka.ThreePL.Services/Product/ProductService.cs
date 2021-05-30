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
            return _unitOfWork.ProductRepository.GetAll();
        }

        public async Task<Dal.Entities.Product> AddProduct(Dal.Entities.Product Product)
        {
            Guard.Argument(Product, nameof(Product)).NotNull();

            await ValidateProduct(Product);

            _unitOfWork.ProductRepository.Insert(Product);
            await _unitOfWork.SaveChanges();

            //Todo send the create new Product to tracking application

            return Product;
        }

        public async Task<Dal.Entities.Product> UpdateProduct(Dal.Entities.Product Product)
        {
            Guard.Argument(Product, nameof(Product)).NotNull();

            await ValidateProduct(Product);

            var currentProduct = await GetProductById(Product.Id);

            var mapper = ServiceMapper.GetMapper();
            mapper.Map(Product, currentProduct);

            await _unitOfWork.SaveChanges();

            //Todo send the update Product details to tracking application

            return currentProduct;
        }

        public async Task<Dal.Entities.Product> GetProductById(long id)
        {
            var Product = await _unitOfWork.ProductRepository.GetAll()
                .Where(s => s.Id == id)     
               
                .FirstOrDefaultAsync();

            if (Product == null)
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

            return Product;
        }

        public async Task SetProductStatus(long id, bool status)
        {
            var currentProduct = await GetProductById(id);
            currentProduct.Active = status;
            await _unitOfWork.SaveChanges();

            //Todo send the status change to tracking application
        }

        private async Task ValidateProduct(Dal.Entities.Product Product)
        {
            var ProductValidator = new ProductValidator();
            var validateResult = await ProductValidator.ValidateAsync(Product);

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
