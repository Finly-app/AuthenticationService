using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistance.Repositories {
    public class UserRepository : IUserRepository {
        private readonly AuthenticationDatabaseContext _context;

        public UserRepository(AuthenticationDatabaseContext context) {
            _context = context;
        }

        public async Task CreateUserAsync(User user) {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public User GetHashedPassword(string username, string email) {
            User user = _context.Users
                   .FirstOrDefault(u => u.Username == username || u.Email == email);

            if (user == null)
                return null;

            return user;
        }

        public async Task<UserExistenceResult> FindByEmailOrUsernameAsync(string email, string username) {
            var result = new UserExistenceResult();

            result.EmailExists = await _context.Users.AnyAsync(u => u.Email == email);
            result.UsernameExists = await _context.Users.AnyAsync(u => u.Username == username);

            return result;
        }

    }
}
