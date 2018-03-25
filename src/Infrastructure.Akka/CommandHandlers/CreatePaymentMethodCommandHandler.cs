using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreatePaymentMethodCommandHandler : IRequestHandler<CreatePaymentMethodCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreatePaymentMethodCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem ?? throw new System.ArgumentNullException(nameof(actorSystem));

        public Task Handle(CreatePaymentMethodCommand message, CancellationToken cancellationToken)
        {
            var paymentMethodActor = _actorSystem.ActorOf(PaymentMethodActor.GetProps(message.PaymentMethodId));
            paymentMethodActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}