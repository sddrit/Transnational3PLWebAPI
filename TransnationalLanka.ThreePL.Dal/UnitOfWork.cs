using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TransnationalLanka.ThreePL.Dal.Core;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Dal
{
    public interface IUnitOfWork
    {
        IRepository<City> CityRepository { get; }
        IRepository<Address> AddressRepository { get; }
        IRepository<Supplier> SupplierRepository { get; }
        Task<IDbContextTransaction> GetTransaction();
        Task SaveChanges();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ThreePlDbContext _context;

        private IRepository<City> _cityRepository;
        private IRepository<Address> _addressRepository;
        private IRepository<Supplier> _supplierRepository;

        public UnitOfWork(ThreePlDbContext context)
        {
            _context = context;
        }

        public IRepository<City> CityRepository
        {
            get
            {
                if (_cityRepository != null) return _cityRepository;
                _cityRepository = new Repository<City>(_context);
                return _cityRepository;
            }
        }

        public IRepository<Address> AddressRepository
        {
            get
            {
                if (_addressRepository != null) return _addressRepository;
                _addressRepository = new Repository<Address>(_context);
                return _addressRepository;
            }
        }

        public IRepository<Supplier> SupplierRepository
        {
            get
            {
                if (_supplierRepository != null) return _supplierRepository;
                _supplierRepository = new Repository<Supplier>(_context);
                return _supplierRepository;
            }
        }

        public async Task<IDbContextTransaction> GetTransaction()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

    }
}
