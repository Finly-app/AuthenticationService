using Authentication.Application.Interfaces;
using Authentication.Mapping;

namespace Authentication.Application.Services {
    public class PolicyService : IPolicyService {
        private readonly IPolicyRepository _policyRepository;

        public PolicyService(IPolicyRepository policyRepository) {
            _policyRepository = policyRepository;
        }

        public async Task BulkCreatePolicies(BulkPolicyCreateDto dto) {
            List<Policy> policies = dto.PolicyNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => new Policy(Guid.NewGuid(), name))
                .ToList();

            await _policyRepository.BulkCreatePolicies(policies);
        }


        public async Task CreateAsync(PolicyDto policyDto) {
            var policy = policyDto.ToPolicyEntity();
            await _policyRepository.CreateAsync(policy);
        }

        public async Task DeleteAsync(Guid id) {
            await _policyRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PolicyDto>> GetAllAsync() {
            var policys = await _policyRepository.GetAllAsync();
            return policys.Select(u => u.ToPolicyDto()).ToList();
        }

        public async Task<PolicyDto> GetByIdAsync(Guid id) {
            var policy = await _policyRepository.GetByIdAsync(id);
            return policy.ToPolicyDto();
        }

        public async Task UpdateAsync(PolicyDto policyDto) {
            var policy = policyDto.ToPolicyEntity();
            await _policyRepository.UpdateAsync(policy);
        }
    }
}
