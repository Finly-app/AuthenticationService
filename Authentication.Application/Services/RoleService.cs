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
            var role = roleDto.ToEntity();
            await _roleRepository.CreateAsync(role);
        }

        public async Task DeleteAsync(Guid id) {
            await _roleRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync() {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(u => u.ToDto()).ToList();
        }

        public async Task<RoleDto> GetByIdAsync(Guid id) {
            var role = await _roleRepository.GetByIdAsync(id);
            return role.ToDto();
        }

        public async Task UpdateAsync(RoleDto roleDto) {
            var role = roleDto.ToEntity();
            await _roleRepository.UpdateAsync(role);
        }
    }
}
