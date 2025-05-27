namespace Authentication.Mapping {
    public static class PolicyMappingExtensions {
        public static PolicyDto ToDto(this Policy policy) {
            if (policy == null) return null;

            return new PolicyDto {
                Id = policy.Id,
                Name = policy.Name,
            };
        }

        public static Policy ToEntity(this PolicyDto policyDto) {
            if (policyDto == null) return null;

            return new Policy(policyDto.Id, policyDto.Name);
        }
    }
}
