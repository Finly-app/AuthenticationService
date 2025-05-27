using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("roles")]
public class RoleController : ControllerBase {
    private readonly IRoleService _roleService;
    public RoleController(IRoleService roleService) {
        _roleService = roleService;
    }
    //TODO: Improve controller with FROM BOdy, look at user service
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _roleService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) {
        var role = await _roleService.GetByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleDto dto) {
        await _roleService.CreateAsync(dto);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, RoleDto dto) {
        if (id != dto.Id) return BadRequest();
        await _roleService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) {
        await _roleService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{roleId}/policies")]
    public async Task<IActionResult> AssignPoliciesToRole(Guid roleId, [FromBody] AssignPoliciesToRoleDto dto) {
        var success = await _roleService.AssignPoliciesToRoleAsync(roleId, dto.PolicyIds);
        return success ? Ok() : NotFound();
    }
}
