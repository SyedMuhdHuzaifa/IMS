namespace DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Not on the entity. The service fills this with a count query so the
        // UI can show how many products are in the category without loading them.
        public int ProductCount { get; set; }
    }
}