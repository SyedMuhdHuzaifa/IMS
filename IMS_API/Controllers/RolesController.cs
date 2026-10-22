using Business.Interfaces;
using Dtos.Roles;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    public class RolesController : BaseApiController
    {
        private readonly IApplicationRoleService _roleService;

        public RolesController(IApplicationRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var roles = await _roleService.GetAllAsync(ct);
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
        {
            var role = await _roleService.GetByIdAsync(id, ct);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken ct = default)
        {
            var createdRole = await _roleService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdRole.Id }, createdRole);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRoleRequest request, CancellationToken ct = default)
        {
            var updatedRole = await _roleService.UpdateAsync(id, request, ct);
            if (updatedRole == null)
            {
                return NotFound();
            }

            return Ok(updatedRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
        {
            var success = await _roleService.DeleteAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
