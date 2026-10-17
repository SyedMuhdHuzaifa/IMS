using System.ComponentModel.DataAnnotations;

namespace DTOs.Customers
{
    public class CreateCustomerRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        // Not included: Id (database-generated) and SalesOrders (managed through sales orders).
    }
}