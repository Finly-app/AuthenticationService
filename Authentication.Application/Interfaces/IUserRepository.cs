using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces {
    public interface IUserRepository {
        Task CreateUserAsync(User user);
        Task<User> FindByIdAsync(Guid id);
        Task UpdateUserAsync(User user);

        User GetHashedPassword(string username, string email);
        Task<UserExistenceResult> FindByEmailOrUsernameAsync(string email, string username);

    }
}
