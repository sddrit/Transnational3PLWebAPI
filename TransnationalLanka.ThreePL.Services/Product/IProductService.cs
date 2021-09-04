using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public interface IProductService
    {
        IQueryable<Dal.Entities.Product> GetProducts();
        IQueryable<Dal.Entities.Product> GetProducts(long supplierId);
        Task<List<UnitOfMeasure>> GetUnitOfMeasures();
        Task<Dal.Entities.Product> AddProduct(Dal.Entities.Product product);
        Task<Dal.Entities.Product> UpdateProduct(Dal.Entities.Product product);
        Task<Dal.Entities.Product> GetProductById(long id);
        Task SetProductStatus(long id, bool status);
        bool IsActiveProduct(Dal.Entities.Product product);

        Task<List<Dal.Entities.Product>> GetProductBySupplierId(long supplierId);
    }
}