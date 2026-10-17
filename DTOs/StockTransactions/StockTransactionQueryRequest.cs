using System.ComponentModel.DataAnnotations;

namespace DTOs.StockTransactions
{
    // Query-string filter for GET /stock-transactions.
    public class StockTransactionQueryRequest
    {
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }

        [StringLength(50)]
        public string? TransactionType { get; set; }

        // Dates are inclusive for the client. The service converts ToDate to an
        // exclusive upper bound by adding one day, so "to 31 Oct" includes 31 Oct.
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        // Ledgers grow fast, so the list is always paged.
        [Range(1, 200)]
        public int PageSize { get; set; } = 50;

        // Not included: sorting. The ledger is always newest first.
    }
}