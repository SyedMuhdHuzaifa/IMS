namespace IMS.Entities
{
    public class StockTransfer
    {
        public int Id { get; set; }
        public int SourceWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public DateTime TransferDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public string? ApprovedByUserId { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public Warehouse? SourceWarehouse { get; set; }
        public Warehouse? DestinationWarehouse { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        public ApplicationUser? ApprovedByUser { get; set; }
        public ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();
    }
}
