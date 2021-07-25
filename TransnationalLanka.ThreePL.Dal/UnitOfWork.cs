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
        IRepository<Product> ProductRepository { get; }
        IRepository<WareHouse> WareHouseRepository { get; }
        IRepository<PurchaseOrder> PurchaseOrdeRepository { get; }
        IRepository<GoodReceivedNote> GoodReceiveNoteRepository { get; }
        IRepository<GoodReceivedNoteItems> GoodReceivedNoteItemsRepository { get; }
        IRepository<ProductStock> ProductStockRepository { get; }
        IRepository<ProductStockAdjustment> ProductStockAdjustmentRepository { get; }
        IRepository<StockTransfer> StockTransferRepository { get; }
        Task<IDbContextTransaction> GetTransaction();
        Task SaveChanges();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ThreePlDbContext _context;

        private IRepository<City> _cityRepository;
        private IRepository<Address> _addressRepository;
        private IRepository<Supplier> _supplierRepository;
        private IRepository<WareHouse> _warehouseRepository;
        private IRepository<Product> _productRepository;
        private IRepository<PurchaseOrder> _purchaseOrdeRepository;
        private IRepository<GoodReceivedNote> _goodReceivedNoteRepository;
        private IRepository<GoodReceivedNoteItems> _goodReceivedNoteItemsRepository;
        private IRepository<ProductStock> _productStockRepository;
        private IRepository<ProductStockAdjustment> _productStockAdjustmentRepository;
        private IRepository<StockTransfer> _stockTranferRepository;

        public UnitOfWork(ThreePlDbContext context)
        {
            _context = context;
        }

        public IRepository<StockTransfer> StockTransferRepository
        {
            get
            {
                return _stockTranferRepository ??= new Repository<StockTransfer>(_context);
            }
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
        public IRepository<Product> ProductRepository
        {
            get
            {
                if (_supplierRepository != null) return _productRepository;
                _productRepository = new Repository<Product>(_context);
                return _productRepository;
            }
        }

        public IRepository<WareHouse> WareHouseRepository
        {
            get
            {
                if (_warehouseRepository != null) return _warehouseRepository;
                _warehouseRepository = new Repository<WareHouse>(_context);
                return _warehouseRepository;
            }
        }

        public IRepository<PurchaseOrder> PurchaseOrdeRepository
        {
            get
            {
                if (_purchaseOrdeRepository != null) return _purchaseOrdeRepository;
                _purchaseOrdeRepository = new Repository<PurchaseOrder>(_context);
                return _purchaseOrdeRepository;
            }
        }

        public IRepository<GoodReceivedNote> GoodReceiveNoteRepository
        {
            get
            {
                if (_goodReceivedNoteRepository != null) return _goodReceivedNoteRepository;
                _goodReceivedNoteRepository = new Repository<GoodReceivedNote>(_context);
                return _goodReceivedNoteRepository;
            }
        }

        public IRepository<GoodReceivedNoteItems> GoodReceivedNoteItemsRepository
        {
            get
            {
                if (_goodReceivedNoteItemsRepository != null) return _goodReceivedNoteItemsRepository;
                _goodReceivedNoteItemsRepository = new Repository<GoodReceivedNoteItems>(_context);
                return _goodReceivedNoteItemsRepository;
            }
        }

        public IRepository<ProductStock> ProductStockRepository
        {
            get
            {
                if (_productStockRepository != null) return _productStockRepository;
                _productStockRepository = new Repository<ProductStock>(_context);
                return _productStockRepository;
            }
        }

        public IRepository<ProductStockAdjustment> ProductStockAdjustmentRepository
        {
            get
            {
                if (_productStockAdjustmentRepository != null) return _productStockAdjustmentRepository;
                _productStockAdjustmentRepository = new Repository<ProductStockAdjustment>(_context);
                return _productStockAdjustmentRepository;
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
