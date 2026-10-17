using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IMS.Entities;

namespace IMS.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Users and Roles are provided by IdentityDbContext (Users, Roles).
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<WarehouseStock> WarehouseStocks { get; set; } = null!;
    public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
    public DbSet<StockTransfer> StockTransfers { get; set; } = null!;
    public DbSet<StockTransferItem> StockTransferItems { get; set; } = null!;
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;
    public DbSet<SalesOrder> SalesOrders { get; set; } = null!;
    public DbSet<SalesOrderItem> SalesOrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.HasMany(e => e.CreatedPurchaseOrders)
                .WithOne(po => po.CreatedByUser)
                .HasForeignKey(po => po.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.CreatedSalesOrders)
                .WithOne(so => so.CreatedByUser)
                .HasForeignKey(so => so.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.CreatedStockTransactions)
                .WithOne(st => st.CreatedByUser)
                .HasForeignKey(st => st.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.CreatedStockTransfers)
                .WithOne(str => str.CreatedByUser)
                .HasForeignKey(str => str.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.ApprovedStockTransfers)
                .WithOne(str => str.ApprovedByUser)
                .HasForeignKey(str => str.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasMany(e => e.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.Name);

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.SellingPrice).HasPrecision(18, 2);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Product_PricesNonNegative",
                "PurchasePrice >= 0 AND SellingPrice >= 0"));

            entity.HasMany(e => e.WarehouseStocks)
                .WithOne(ws => ws.Product)
                .HasForeignKey(ws => ws.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.StockTransactions)
                .WithOne(st => st.Product)
                .HasForeignKey(st => st.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.PurchaseOrderItems)
                .WithOne(poi => poi.Product)
                .HasForeignKey(poi => poi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SalesOrderItems)
                .WithOne(soi => soi.Product)
                .HasForeignKey(soi => soi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.StockTransferItems)
                .WithOne(sti => sti.Product)
                .HasForeignKey(sti => sti.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);

            entity.HasMany(e => e.PurchaseOrders)
                .WithOne(po => po.Supplier)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);

            entity.HasMany(e => e.SalesOrders)
                .WithOne(so => so.Customer)
                .HasForeignKey(so => so.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasMany(e => e.WarehouseStocks)
                .WithOne(ws => ws.Warehouse)
                .HasForeignKey(ws => ws.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.StockTransactions)
                .WithOne(st => st.Warehouse)
                .HasForeignKey(st => st.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SourceStockTransfers)
                .WithOne(str => str.SourceWarehouse)
                .HasForeignKey(str => str.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.DestinationStockTransfers)
                .WithOne(str => str.DestinationWarehouse)
                .HasForeignKey(str => str.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.PurchaseOrders)
                .WithOne(po => po.DestinationWarehouse)
                .HasForeignKey(po => po.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SalesOrders)
                .WithOne(so => so.SourceWarehouse)
                .HasForeignKey(so => so.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<WarehouseStock>(entity =>
        {
          entity.HasIndex(e => new { e.WarehouseId, e.ProductId }).IsUnique();

            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_WarehouseStock_QtyNonNegative",
                "QuantityOnHand >= 0"));
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => new { e.ProductId, e.WarehouseId, e.TransactionDate });
        });

        
        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_StockTransfer_DifferentWarehouses",
                "SourceWarehouseId <> DestinationWarehouseId"));

            entity.HasMany(e => e.StockTransferItems)
                .WithOne(sti => sti.StockTransfer)
                .HasForeignKey(sti => sti.StockTransferId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StockTransferItem>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_StockTransferItem_QtyPositive",
                "Quantity > 0"));
        });


        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasIndex(e => e.OrderDate);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_PurchaseOrder_TotalNonNegative",
                "TotalAmount >= 0"));

            entity.HasMany(e => e.PurchaseOrderItems)
                .WithOne(poi => poi.PurchaseOrder)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_PurchaseOrderItem_QtyPositive",
                "Quantity > 0"));
        });

        
        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasIndex(e => e.OrderDate);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_SalesOrder_TotalNonNegative",
                "TotalAmount >= 0"));

            entity.HasMany(e => e.SalesOrderItems)
                .WithOne(soi => soi.SalesOrder)
                .HasForeignKey(soi => soi.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SalesOrderItem>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_SalesOrderItem_QtyPositive",
                "Quantity > 0"));
        });
    }
}