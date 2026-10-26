namespace IMS.Entities
{
    public class StockTransaction
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int QuantityChanged { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public int? ReferenceId { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;

        public Warehouse? Warehouse { get; set; }
        public Product? Product { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
    }
}
