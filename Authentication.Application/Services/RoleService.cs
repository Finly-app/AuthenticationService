using Authentication.Mapping;

namespace Authentication.Application.Services {
    public class RoleService : IRoleService {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository) {
            _roleRepository = roleRepository;
        }

        public async Task<bool> AssignPoliciesToRoleAsync(Guid roleId, List<Guid> policyIds) {
            return await _roleRepository.AssignPoliciesToRoleAsync(roleId, policyIds);
        }

        public async Task CreateAsync(RoleDto roleDto) {
            var role = roleDto.ToRoleEntity();
            await _roleRepository.CreateAsync(role);
        }

        public async Task<bool> CreateRoleInheritanceAsync(Guid parentRoleId, Guid childRoleId) {
            return await _roleRepository.CreateRoleInheritanceAsync(parentRoleId, childRoleId);
        }

        public async Task DeleteAsync(Guid id) {
            await _roleRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync() {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(u => u.ToRoleDto()).ToList();
        }
        public async Task<IEnumerable<PolicyDto>> GetAllInheritedPoliciesAsync(Guid roleId) {
            var policies = await _roleRepository.GetAllInheritedPoliciesAsync(roleId);
            return policies.Select(p => p.ToPolicyDto());
        }

        public async Task<RoleDto> GetByIdAsync(Guid id) {
            var role = await _roleRepository.GetByIdAsync(id);
            return role.ToRoleDto();
        }

        public async Task<List<PolicyDto>> GetRolePoliciesAsync(Guid roleId) {
            var policies = await _roleRepository.GetRolePoliciesAsync(roleId);
            return policies.Select(p => p.ToPolicyDto()).ToList();
        }

        public async Task<RoleTreeDto> GetRoleTreeAsync(Guid rootRoleId) {
            return await BuildTreeRecursive(rootRoleId);
        }

        private async Task<RoleTreeDto> BuildTreeRecursive(Guid roleId) {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) return null;

            var children = await _roleRepository.GetChildRoleIdsAsync(roleId);

            var dto = new RoleTreeDto {
                Id = role.Id,
                Name = role.Name
            };

            foreach (var childId in children) {
                var childTree = await BuildTreeRecursive(childId);
                if (childTree != null) dto.Children.Add(childTree);
            }

            return dto;
        }


        public async Task UpdateAsync(RoleDto roleDto) {
            var role = roleDto.ToRoleEntity();
            await _roleRepository.UpdateAsync(role);
        }

        public async Task<List<Policy>> GetAllPoliciesForRoleAndInheritedAsync(Guid roleId) {
            var roleIds = await _roleRepository.GetAllDescendantRoleIdsAsync(roleId);
            roleIds.Add(roleId); 

            return await _roleRepository.GetPoliciesForRolesAsync(roleIds);
        }

        public async Task<bool> RemovePolicyFromRoleAsync(Guid roleId, Guid policyId) {
            return await _roleRepository.RemovePolicyFromRoleAsync(roleId, policyId);
        }

        public List<Policy> GetAllPoliciesForRoleAndInherited(Guid roleId) {
            var roleIds = _roleRepository.GetAllDescendantRoleIdsAsync(roleId).GetAwaiter().GetResult();
            roleIds.Add(roleId);
            return _roleRepository.GetPoliciesForRolesAsync(roleIds).GetAwaiter().GetResult();
        }
    }
}
