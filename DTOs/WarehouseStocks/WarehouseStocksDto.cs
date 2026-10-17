namespace DTOs.WarehouseStocks
{
    // One product's balance in one warehouse. Read-only.
    public class WarehouseStockDto
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public int QuantityOnHand { get; set; }

        // Not on the entity. Shown so users can see the total without
        // a second call. Always 0 or more, because the database enforces it.
    }
}