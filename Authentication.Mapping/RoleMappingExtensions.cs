namespace Authentication.Mapping {
    public static class RoleMappingExtensions {
        public static RoleDto ToRoleDto(this Role role) {
            if (role == null) return null;

            return new RoleDto {
                Id = role.Id,
                Name = role.Name
            };
        }

        public static Role ToRoleEntity(this RoleDto roleDto) {
            if (roleDto == null) return null;

            return new Role(roleDto.Id, roleDto.Name);
        }
    }
}
