namespace IMS.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int DestinationWarehouseId { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;

        public Supplier? Supplier { get; set; }
        public Warehouse? DestinationWarehouse { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}
