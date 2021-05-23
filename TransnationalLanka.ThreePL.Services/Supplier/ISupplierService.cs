using System.Linq;
using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Supplier
{
    public interface ISupplierService
    {
        IQueryable<Dal.Entities.Supplier> GetSuppliers();
        Task<Dal.Entities.Supplier> AddSupplier(Dal.Entities.Supplier supplier);
        Task<Dal.Entities.Supplier> UpdateSupplier(Dal.Entities.Supplier supplier);
        Task<Dal.Entities.Supplier> GetSupplierById(long id);
    }
}