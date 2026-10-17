using System.ComponentModel.DataAnnotations;

namespace DTOs.SalesOrders
{
    public class CreateSalesOrderRequest
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Range(1, int.MaxValue)]
        public int SourceWarehouseId { get; set; }

        // Defaults to today on the server if omitted.
        public DateTime? OrderDate { get; set; }

        [Required]
        [MinLength(1)]
        public IReadOnlyList<SalesOrderItemRequest> Items { get; set; } = Array.Empty<SalesOrderItemRequest>();

        // Not included: Id, Status (new orders start as Draft), TotalAmount (computed),
        // and CreatedByUserId (taken from the authenticated user).
    }
}