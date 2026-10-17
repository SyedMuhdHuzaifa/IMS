namespace DTOs.PurchaseOrders
{
    // Lightweight row for list screens. No items are loaded.
    public class PurchaseOrderSummaryDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string DestinationWarehouseName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }
}