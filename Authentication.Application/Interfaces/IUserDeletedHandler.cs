using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Interfaces {
    public interface IUserDeletedHandler {
        Task HandleAsync(UserDeletedEvent message, string secret, IProducer<Null, string> producer);
    }
}
