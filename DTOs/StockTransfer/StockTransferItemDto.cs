namespace DTOs.StockTransfers
{
    public class StockTransferItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // Informational only: stock at the source warehouse when the response was built.
        // It can change before the transfer completes, so the service must check again.
        public int AvailableAtSource { get; set; }
    }
}