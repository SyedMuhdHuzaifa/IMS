namespace DTOs.StockTransactions
{
    // Read-only ledger row. Rows are never edited after they are written.
    public class StockTransactionDto
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }

        // Positive adds stock, negative removes it.
        public int QuantityChanged { get; set; }

        // For example "PurchaseReceipt", "SalesShipment", "TransferOut", "TransferIn", "Adjustment".
        public string TransactionType { get; set; } = string.Empty;

        // Points to a different record depending on TransactionType
        // (purchase order, sales order, transfer, or none for adjustments).
        public int? ReferenceId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;

        // Display only.
        public string CreatedByUserName { get; set; } = string.Empty;
    }
}