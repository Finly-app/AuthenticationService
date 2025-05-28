using Authentication.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistance.Repositories {
    public class PolicyRepository : IPolicyRepository {
        private readonly AuthenticationDatabaseContext _context;

        public PolicyRepository(AuthenticationDatabaseContext context) {
            _context = context;
        }

        public async Task BulkCreatePolicies(List<Policy> policies) {
            _context.Policies.AddRange(policies);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(Policy policy) {
            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();
        }

        //TODO: Improve error handling by adding bool back
        //TODO: On delete policy, also remove the policies attached
        public async Task DeleteAsync(Guid id) {
            var policy = await _context.Policies.FindAsync(id);

            _context.Policies.Remove(policy);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Policy>> GetAllAsync() {
            return await _context.Policies.ToListAsync();
        }

        public async Task<Policy> GetByIdAsync(Guid id) {
            return await _context.Policies.FindAsync(id);
        }

        //TODO: Improve error handling by adding bool back
        public async Task UpdateAsync(Policy policy) {
            var existingPolicy = await _context.Policies.FindAsync(policy.Id);

            var createdAt = existingPolicy.CreatedAt;
            _context.Entry(existingPolicy).CurrentValues.SetValues(policy);
            existingPolicy.CreatedAt = createdAt;

            await _context.SaveChangesAsync();
        }
    }
}
