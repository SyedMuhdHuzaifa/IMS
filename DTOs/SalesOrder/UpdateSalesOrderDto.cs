using System.ComponentModel.DataAnnotations;

namespace DTOs.SalesOrders
{
    // Allowed only while the order is in Draft.
    public class UpdateSalesOrderRequest
    {
        // Id comes from the route.

        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Range(1, int.MaxValue)]
        public int SourceWarehouseId { get; set; }

        public DateTime OrderDate { get; set; }

        // The full list replaces the existing items.
        [Required]
        [MinLength(1)]
        public IReadOnlyList<SalesOrderItemRequest> Items { get; set; } = Array.Empty<SalesOrderItemRequest>();
    }
}