using DTOs.SalesOrder;

namespace DTOs.SalesOrders
{
    // Full detail view, including line items.
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Computed by the service from the items. Never set by the client.
        public decimal TotalAmount { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public int SourceWarehouseId { get; set; }
        public string SourceWarehouseName { get; set; } = string.Empty;

        // Display only.
        public string CreatedByUserName { get; set; } = string.Empty;

        public IReadOnlyList<SalesOrderItemDto> Items { get; set; } = Array.Empty<SalesOrderItemDto>();
    }
}