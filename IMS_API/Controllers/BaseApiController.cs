
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected string GetCurrentUserId()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? "system";
        }

        protected string? GetCurrentUserName()
        {
            return User.Identity?.Name;
        }
    }
}
