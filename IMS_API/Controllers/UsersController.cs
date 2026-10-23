using Business.Interfaces;
using DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UsersController : BaseApiController
    {
        private readonly IApplicationUserService _userService;

        public UsersController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false, CancellationToken ct = default)
        {
            var users = await _userService.GetAllAsync(includeInactive, ct);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct = default)
        {
            var createdUser = await _userService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
        {
            var updatedUser = await _userService.UpdateAsync(id, request, ct);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> SetRoles(string id, [FromBody] IReadOnlyList<string> roleNames, CancellationToken ct = default)
        {
            var updatedUser = await _userService.SetRolesAsync(id, roleNames, ct);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id, CancellationToken ct = default)
        {
            var success = await _userService.DeactivateAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(string id, CancellationToken ct = default)
        {
            var success = await _userService.ReactivateAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
