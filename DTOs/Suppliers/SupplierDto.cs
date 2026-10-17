namespace DTOs.Suppliers
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Not on the entity. The service fills this with a count query so the
        // UI can show purchasing activity without loading every order.
        public int PurchaseOrderCount { get; set; }
    }
}