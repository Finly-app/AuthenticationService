using Confluent.Kafka;

namespace Authentication.Application.Interfaces {
    public interface IUserUpdatedHandler {
        Task HandleAsync(UserUpdatedEvent message, string secret, IProducer<Null, string> producer);
    }
}
