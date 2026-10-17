namespace DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }

        // Category: the ID for editing, the name for display.
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Not on the entity. Computed by the service from WarehouseStock
        // across all warehouses.
        public int TotalOnHand { get; set; }

        // TotalOnHand <= ReorderLevel for active products.
        public bool IsLowStock { get; set; }
    }
}