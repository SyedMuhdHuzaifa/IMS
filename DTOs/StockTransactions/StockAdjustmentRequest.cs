using System.ComponentModel.DataAnnotations;

namespace DTOs.StockTransactions
{
    // Manual correction after a stock count. The only way a client creates a ledger row directly.
    public class StockAdjustmentRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }

        // Positive adds stock, negative removes it. Zero is rejected by the service.
        [Range(-1000000, 1000000)]
        public int QuantityChange { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        // Not included: Id, TransactionType (always "Adjustment" for this endpoint),
        // TransactionDate (server time), and CreatedByUserId (authenticated user).
    }
}