namespace DTOs.Warehouses
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Description { get; set; }

        // Changed only through the activate and deactivate actions.
        public bool IsActive { get; set; }
    }
}