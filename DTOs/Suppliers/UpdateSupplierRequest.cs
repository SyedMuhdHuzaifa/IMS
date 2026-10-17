using System.ComponentModel.DataAnnotations;

namespace DTOs.Suppliers
{
    public class UpdateSupplierRequest
    {
        // Id comes from the route.

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
    }
}