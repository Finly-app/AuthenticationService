using Authentication.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("users")]
public class UserController : Controller {
    private readonly IUserService _userService;
    public UserController(IUserService userService) {
        _userService = userService;
    }

    [Authorize(Policy = "users:roles:read")]
    [HttpGet("{userId}/role")]
    public async Task<IActionResult> GetUserRole(Guid userId) {
        var role = await _userService.GetUserRoleAsync(userId);
        return role == null ? NotFound() : Ok(role);
    }

    [Authorize(Policy = "users:roles:assign")]
    [HttpPut("{userId}/role")]
    public async Task<IActionResult> AssignRoleToUser(Guid userId, [FromBody] UserRoleDto dto) {
        var success = await _userService.AssignRoleToUserAsync(userId, dto.RoleId);
        return success ? Ok() : NotFound();
    }

    [Authorize(Policy = "users:policies:read")]
    [HttpGet("{userId}/policies")]
    public async Task<IActionResult> GetPolicies(Guid userId) =>
        Ok(await _userService.GetUserPoliciesAsync(userId));

    [Authorize(Policy = "users:policies:assign")]
    [HttpPost("{userId}/policies")]
    public async Task<IActionResult> AssignPolicies(Guid userId, [FromBody] BulkUserPolicyDto dto) {
        var success = await _userService.AssignUserPoliciesAsync(userId, dto.PolicyIds);
        return success ? Ok() : NotFound();
    }

    [Authorize(Policy = "users:policies:remove")]
    [HttpDelete("{userId}/policies/{policyId}")]
    public async Task<IActionResult> RemovePolicy(Guid userId, Guid policyId) {
        var success = await _userService.RemoveUserPolicyAsync(userId, policyId);
        return success ? NotFound() : NoContent();
    }

    [Authorize(Policy = "users:update")]
    [HttpPut("{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid userId) {
        var result = await _userService.DeactivateUserAsync(userId);
        return result ? Ok() : NotFound();
    }

    [Authorize(Policy = "users:update")]
    [HttpPut("{userId}/activate")]
    public async Task<IActionResult> ActivateUser(Guid userId) {
        var result = await _userService.ActivateUserAsync(userId);
        return result ? Ok() : NotFound();
    }
}
