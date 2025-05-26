using Authentication.Domain.Entities;
using Authentication.Persistance.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistance {
    public class AuthenticationDatabaseContext : DbContext {
        public AuthenticationDatabaseContext(DbContextOptions<AuthenticationDatabaseContext> options) : base(options) { }

        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePolicy> RolePolicies { get; set; }
        public DbSet<UserPolicy> UserPolicies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new PolicyConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RolePolicyConfiguration());
            modelBuilder.ApplyConfiguration(new UserPolicyConfiguration());

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            var entries = ChangeTracker
            .Entries()
                .Where(e => e.Entity is Auditable &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries) {
                var entity = (Auditable)entry.Entity;

                if (entry.State == EntityState.Added)
                    entity.CreatedAt = DateTime.UtcNow;

                entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
