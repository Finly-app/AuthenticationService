﻿using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistance {
    public class AuthenticationDatabaseContext : DbContext {
        public AuthenticationDatabaseContext(DbContextOptions<AuthenticationDatabaseContext> options) : base(options) { }

        public DbSet<Token> Tokens { get; set; }

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
