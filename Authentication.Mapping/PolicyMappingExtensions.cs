namespace Authentication.Mapping {
    public static class PolicyMappingExtensions {
        public static PolicyDto ToPolicyDto(this Policy policy) {
            if (policy == null) return null;

            return new PolicyDto {
                Id = policy.Id,
                Name = policy.Name,
            };
        }

        public static Policy ToPolicyEntity(this PolicyDto policyDto) {
            if (policyDto == null) return null;

            return new Policy(policyDto.Id, policyDto.Name);
        }
    }
}
