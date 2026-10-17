using DTOs.PurchaseOrder;

namespace DTOs.PurchaseOrders
{
    // Full detail view, including line items.
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Computed by the service from the items. Never set by the client.
        public decimal TotalAmount { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;

        public int DestinationWarehouseId { get; set; }
        public string DestinationWarehouseName { get; set; } = string.Empty;

        // Display only. Taken from ApplicationUser.UserName or FullName.
        public string CreatedByUserName { get; set; } = string.Empty;

        public IReadOnlyList<PurchaseOrderItemDto> Items { get; set; } = Array.Empty<PurchaseOrderItemDto>();
    }
}