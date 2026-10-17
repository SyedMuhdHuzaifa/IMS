namespace IMS.Entities
{
    public class StockTransferItem
    {
        public int Id { get; set; }
        public int StockTransferId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Navigation properties
        public StockTransfer? StockTransfer { get; set; }
        public Product? Product { get; set; }
    }
}
