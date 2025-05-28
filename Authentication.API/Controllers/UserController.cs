using Authentication.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers {
    public class UserController : Controller {
        private readonly IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpGet("{userId}/role")]
        public async Task<IActionResult> GetUserRole(Guid userId) {
            var role = await _userService.GetUserRoleAsync(userId);
            return role == null ? NotFound() : Ok(role);
        }

        [HttpPost("{userId}/role")]
        public async Task<IActionResult> AssignRoleToUser(Guid userId, [FromBody] UserRoleDto dto) {
            var success = await _userService.AssignRoleToUserAsync(userId, dto.RoleId);
            return success ? Ok() : NotFound();
        }

        [HttpPut("{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid userId, [FromBody] UserRoleDto dto) {
            var success = await _userService.UpdateUserRoleAsync(userId, dto.RoleId);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("{userId}/policies")]
        public async Task<IActionResult> GetPolicies(Guid userId) =>
            Ok(await _userService.GetUserPoliciesAsync(userId));

        [HttpPost("{userId}/policies")]
        public async Task<IActionResult> AssignPolicies(Guid userId, [FromBody] BulkUserPolicyDto dto) {
            var success = await _userService.AssignUserPoliciesAsync(userId, dto.PolicyIds);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("{userId}/policies/{policyId}")]
        public async Task<IActionResult> RemovePolicy(Guid userId, Guid policyId) {
            var success = await _userService.RemoveUserPolicyAsync(userId, policyId);
            return success ? NotFound() : NoContent();
        }

    }
}
