using System.Linq;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Product
{
    public interface IProductService
    {
        IQueryable<Dal.Entities.Product> GetProducts();
        Task<Dal.Entities.Product> AddProduct(Dal.Entities.Product product);
        Task<Dal.Entities.Product> UpdateProduct(Dal.Entities.Product product);
        Task<Dal.Entities.Product> GetProductById(long id);
        Task SetProductStatus(long id, bool status);
        bool IsActiveProduct(Dal.Entities.Product product);
    }
}