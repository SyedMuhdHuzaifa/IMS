using Microsoft.EntityFrameworkCore.Storage;
using Data.Interfaces;
using Data;
using IMS.Data;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        // Injected repositories
        private readonly IApplicationRoleRepository _applicationRoles;
        private readonly IApplicationUserRepositery _applicationUsers;
        private readonly ICategoryRepository _categories;
        private readonly ICustomerRepository _customers;
        private readonly IProductRepository _products;
        private readonly ISupplierRepository _suppliers;
        private readonly IWarehouseRepository _warehouses;
        private readonly IWarehouseStockRepository _warehouseStocks;
        private readonly IStockTransactionRepository _stockTransactions;
        private readonly IStockTransferRepository _stockTransfers;
        private readonly IPurchaseOrderRepository _purchaseOrders;
        private readonly ISalesOrderRepository _salesOrders;

        public UnitOfWork(
            AppDbContext context,
            IApplicationRoleRepository applicationRoles,
            IApplicationUserRepositery applicationUsers,
            ICategoryRepository categories,
            ICustomerRepository customers,
            IProductRepository products,
            ISupplierRepository suppliers,
            IWarehouseRepository warehouses,
            IWarehouseStockRepository warehouseStocks,
            IStockTransactionRepository stockTransactions,
            IStockTransferRepository stockTransfers,
            IPurchaseOrderRepository purchaseOrders,
            ISalesOrderRepository salesOrders)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _applicationRoles = applicationRoles ?? throw new ArgumentNullException(nameof(applicationRoles));
            _applicationUsers = applicationUsers ?? throw new ArgumentNullException(nameof(applicationUsers));
            _categories = categories ?? throw new ArgumentNullException(nameof(categories));
            _customers = customers ?? throw new ArgumentNullException(nameof(customers));
            _products = products ?? throw new ArgumentNullException(nameof(products));
            _suppliers = suppliers ?? throw new ArgumentNullException(nameof(suppliers));
            _warehouses = warehouses ?? throw new ArgumentNullException(nameof(warehouses));
            _warehouseStocks = warehouseStocks ?? throw new ArgumentNullException(nameof(warehouseStocks));
            _stockTransactions = stockTransactions ?? throw new ArgumentNullException(nameof(stockTransactions));
            _stockTransfers = stockTransfers ?? throw new ArgumentNullException(nameof(stockTransfers));
            _purchaseOrders = purchaseOrders ?? throw new ArgumentNullException(nameof(purchaseOrders));
            _salesOrders = salesOrders ?? throw new ArgumentNullException(nameof(salesOrders));
        }

        /// <summary>
        /// Gets the application role repository.
        /// </summary>
        public IApplicationRoleRepository ApplicationRoles => _applicationRoles;

        /// <summary>
        /// Gets the application user repository.
        /// </summary>
        public IApplicationUserRepositery ApplicationUsers => _applicationUsers;

        /// <summary>
        /// Gets the category repository.
        /// </summary>
        public ICategoryRepository Categories => _categories;

        /// <summary>
        /// Gets the customer repository.
        /// </summary>
        public ICustomerRepository Customers => _customers;

        /// <summary>
        /// Gets the product repository.
        /// </summary>
        public IProductRepository Products => _products;

        /// <summary>
        /// Gets the supplier repository.
        /// </summary>
        public ISupplierRepository Suppliers => _suppliers;

        /// <summary>
        /// Gets the warehouse repository.
        /// </summary>
        public IWarehouseRepository Warehouses => _warehouses;

        /// <summary>
        /// Gets the warehouse stock repository.
        /// </summary>
        public IWarehouseStockRepository WarehouseStocks => _warehouseStocks;

        /// <summary>
        /// Gets the stock transaction repository.
        /// </summary>
        public IStockTransactionRepository StockTransactions => _stockTransactions;

        /// <summary>
        /// Gets the stock transfer repository.
        /// </summary>
        public IStockTransferRepository StockTransfers => _stockTransfers;

        /// <summary>
        /// Gets the purchase order repository.
        /// </summary>
        public IPurchaseOrderRepository PurchaseOrders => _purchaseOrders;

        /// <summary>
        /// Gets the sales order repository.
        /// </summary>
        public ISalesOrderRepository SalesOrders => _salesOrders;

        /// <summary>
        /// Gets a generic repository for the specified entity type.
        /// </summary>
        public IGenericRepository<T> Repository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        /// <summary>
        /// Saves all changes to the database in a single transaction.
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                return await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                // Log the exception here if using a logger
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
        }

        /// <summary>
        /// Rolls back all pending changes without saving.
        /// </summary>
        public async Task RollbackAsync(CancellationToken ct = default)
        {
            await _context.Database.RollbackTransactionAsync(ct);
        }

        /// <summary>
        /// Begins a database transaction for atomic operations.
        /// </summary>
        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            try
            {
                await SaveChangesAsync(ct);
                if (_transaction is not null)
                {
                    await _transaction.CommitAsync(ct);
                }
            }
            catch
            {
                await RollbackAsync(ct);
                throw;
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Disposes the unit of work and any active transactions.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
            }
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
