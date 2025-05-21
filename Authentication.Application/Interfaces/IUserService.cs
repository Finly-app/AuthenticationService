using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces {
    public interface IUserService {
        Task<User> FindByIdAsync(Guid userId);
        Task GenerateEmailConfirmationAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
