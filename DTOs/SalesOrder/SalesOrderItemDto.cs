namespace DTOs.SalesOrders
{
    public class SalesOrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // The price agreed on the order, not the product's current SellingPrice.
        public decimal UnitPrice { get; set; }

        // Computed as Quantity * UnitPrice.
        public decimal LineTotal { get; set; }
    }
}