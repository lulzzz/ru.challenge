using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreatePaymentMethodCommandHandler : IRequestHandler<CreatePaymentMethodCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreatePaymentMethodCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem;

        public Task Handle(CreatePaymentMethodCommand message, CancellationToken cancellationToken)
        {
            var paymentMethodId = Guid.NewGuid();
            var paymentMethodActor = _actorSystem.ActorOf(PaymentMethodActor.GetProps(paymentMethodId));
            paymentMethodActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}