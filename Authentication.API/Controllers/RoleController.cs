using Authentication.Application.Interfaces;
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

    [HttpGet("{roleId}/policies/inherited")]
    public async Task<IActionResult> GetInheritedPolicies(Guid roleId) {
        var policies = await _roleService.GetAllInheritedPoliciesAsync(roleId);
        return Ok(policies);
    }

    [HttpGet("{roleId}/policies")]
    public async Task<IActionResult> GetRolePolicies(Guid roleId) {
        var policies = await _roleService.GetRolePoliciesAsync(roleId);
        return Ok(policies);
    }

    [HttpPost("inheritance")]
    public async Task<IActionResult> CreateRoleInheritance([FromBody] CreateRoleInheritanceDto dto) {
        var success = await _roleService.CreateRoleInheritanceAsync(dto.ParentRoleId, dto.ChildRoleId);
        return success ? Ok() : BadRequest("Invalid or circular link.");
    }

    [HttpGet("{roleId}/tree")]
    public async Task<IActionResult> GetRoleTree(Guid roleId) {
        var tree = await _roleService.GetRoleTreeAsync(roleId);
        return tree == null ? NotFound() : Ok(tree);
    }

    [HttpDelete("{roleId}/policies/{policyId}")]
    public async Task<IActionResult> RemovePolicyFromRole(Guid roleId, Guid policyId) {
        var success = await _roleService.RemovePolicyFromRoleAsync(roleId, policyId);
        return success ? NoContent() : NotFound();
    }
}
