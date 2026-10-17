namespace DTOs.SalesOrders
{
    // Lightweight row for list screens. No items are loaded.
    public class SalesOrderSummaryDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string SourceWarehouseName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }
}