using Confluent.Kafka;

public interface IUserCreatedHandler {
    Task HandleAsync(UserCreatedEvent message, string secret, IProducer<Null, string> producer);
}
