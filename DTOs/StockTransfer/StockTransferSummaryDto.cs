namespace DTOs.StockTransfers
{
    // Lightweight row for list screens. No items are loaded.
    public class StockTransferSummaryDto
    {
        public int Id { get; set; }
        public DateTime TransferDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SourceWarehouseName { get; set; } = string.Empty;
        public string DestinationWarehouseName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}