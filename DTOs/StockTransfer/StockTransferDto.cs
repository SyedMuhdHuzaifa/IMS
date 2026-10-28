namespace DTOs.StockTransfers
{
    // Full detail view, including line items.
    public class StockTransferDto
    {
        public int Id { get; set; }
        public DateTime TransferDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public int SourceWarehouseId { get; set; }
        public string SourceWarehouseName { get; set; } = string.Empty;

        public int DestinationWarehouseId { get; set; }
        public string DestinationWarehouseName { get; set; } = string.Empty;

        // Display only. Null until someone approves the transfer.
        public string? CreatedByUserName { get; set; }
        public string? ApprovedByUserName { get; set; }

        // Set by the server when the transfer completes.
        public DateTime? CompletedAt { get; set; }

        public IReadOnlyList<StockTransferItemDto> Items { get; set; } = Array.Empty<StockTransferItemDto>();
    }
}