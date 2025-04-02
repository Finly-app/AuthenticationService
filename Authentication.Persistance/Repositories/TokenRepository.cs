using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;

namespace Authentication.Persistance.Repositories {
    public class TokenRepository : ITokenRepository {
        private readonly AuthenticationDatabaseContext _context;

        public TokenRepository(AuthenticationDatabaseContext context) {
            _context = context;
        }

        public async Task StoreToken(Token token) {
            _context.Add(token);
            await _context.SaveChangesAsync();
        }
    }
}
