namespace Data.Interfaces
{
    /// <summary>
    /// Unit of Work pattern - coordinates multiple repositories into a single transaction context.
    /// Enables atomic operations across multiple entities and ensures data consistency.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Gets the generic repository for the specified entity type.
        /// </summary>
        IGenericRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Gets the application role repository.
        /// </summary>
        IApplicationRoleRepository ApplicationRoles { get; }

        /// <summary>
        /// Gets the application user repository.
        /// </summary>
        IApplicationUserRepositery ApplicationUsers { get; }

        /// <summary>
        /// Gets the category repository.
        /// </summary>
        ICategoryRepository Categories { get; }

        /// <summary>
        /// Gets the customer repository.
        /// </summary>
        ICustomerRepository Customers { get; }

        /// <summary>
        /// Gets the product repository.
        /// </summary>
        IProductRepository Products { get; }

        /// <summary>
        /// Gets the supplier repository.
        /// </summary>
        ISupplierRepository Suppliers { get; }

        /// <summary>
        /// Gets the warehouse repository.
        /// </summary>
        IWarehouseRepository Warehouses { get; }

        /// <summary>
        /// Gets the warehouse stock repository.
        /// </summary>
        IWarehouseStockRepository WarehouseStocks { get; }

        /// <summary>
        /// Gets the stock transaction repository.
        /// </summary>
        IStockTransactionRepository StockTransactions { get; }

        /// <summary>
        /// Gets the stock transfer repository.
        /// </summary>
        IStockTransferRepository StockTransfers { get; }

        /// <summary>
        /// Gets the purchase order repository.
        /// </summary>
        IPurchaseOrderRepository PurchaseOrders { get; }

        /// <summary>
        /// Gets the sales order repository.
        /// </summary>
        ISalesOrderRepository SalesOrders { get; }

        /// <summary>
        /// Saves all changes to the database in a single transaction.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The number of entities changed.</returns>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// Rolls back all pending changes without saving.
        /// </summary>
        Task RollbackAsync(CancellationToken ct = default);

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        Task CommitTransactionAsync(CancellationToken ct = default);
    }
}
