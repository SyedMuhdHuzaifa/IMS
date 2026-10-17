using System.ComponentModel.DataAnnotations;

namespace DTOs.StockTransfers
{
    // Shared by create and update.
    public class StockTransferItemRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        // Quantity is in the product's base unit. Zero or negative is rejected by the database.
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Not included: Id and StockTransferId (set from the parent transfer).
    }
}