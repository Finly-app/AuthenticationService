using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces {
    public interface IUserRepository {
        Task CreateUserAsync(User user);
        User GetHashedPassword(string username, string email);
    }
}
