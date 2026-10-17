namespace DTOs.WarehouseStocks
{
    // One row of the low-stock report. Stock is totalled across all warehouses,
    // because that's how ReorderLevel is defined on Product.
    public class LowStockItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int ReorderLevel { get; set; }
        public int TotalOnHand { get; set; }

        // Computed as ReorderLevel - TotalOnHand. Tells purchasing how much to order
        // to get back to the reorder level. Never negative in this report.
        public int ShortBy { get; set; }
    }
}