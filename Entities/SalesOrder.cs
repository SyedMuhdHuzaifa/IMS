namespace IMS.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int SourceWarehouseId { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public Customer? Customer { get; set; }
        public Warehouse? SourceWarehouse { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
    }
}
