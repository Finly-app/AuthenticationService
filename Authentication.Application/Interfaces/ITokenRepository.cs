using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces {
    public interface ITokenRepository {
        Task StoreToken(Token token);
    }
}
