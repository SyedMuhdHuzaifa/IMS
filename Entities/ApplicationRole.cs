using Microsoft.AspNetCore.Identity;

namespace IMS.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}