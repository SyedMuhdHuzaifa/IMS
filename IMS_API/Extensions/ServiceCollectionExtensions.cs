using Business.Interfaces;
using Business.Services;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IMS_API.Extensions
{
    /// <summary>
    /// Extension methods for registering all IMS services and repositories in the DI container.
    /// Use in Program.cs: builder.Services.AddImsServices(configuration);
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all business services and repositories in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration (optional).</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddImsServices(this IServiceCollection services, IConfiguration? configuration = null)
        {
            // Register specialized repositories first (required by UnitOfWork)
            services.AddScoped<IApplicationRoleRepository, ApplicationRoleRepository>();
            services.AddScoped<IApplicationUserRepositery, ApplicationUserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IWareHouseRepository, WarehouseRepository>();
            services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();
            services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
            services.AddScoped<IStockTransferRepository, StockTransferRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

            // Register generic repository factory
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register Unit of Work (coordinates all repositories)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register authentication services
            services.AddScoped<IApplicationRoleService, ApplicationRoleService>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();

            // Register master data services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IWarehouseService, WarehouseService>();

            // Register inventory services
            services.AddScoped<IWarehouseStockService, WarehouseStockService>();
            services.AddScoped<IStockTransactionService, StockTransactionService>();

            // Register order services
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<IPurchaseOrderItemService, PurchaseOrderItemService>();
            services.AddScoped<ISalesOrderService, SalesOrderService>();
            services.AddScoped<ISalesOrderItemsService, SalesOrderItemService>();

            // Register stock transfer services
            services.AddScoped<IStockTransferService, StockTransferService>();
            services.AddScoped<IStockTransferItemService, StockTransferService>(); // StockTransferService implements both

            return services;
        }
    }
}
