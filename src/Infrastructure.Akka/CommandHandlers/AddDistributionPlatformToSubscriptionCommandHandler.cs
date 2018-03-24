using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class AddDistributionPlatformToSubscriptionCommandHandler : IRequestHandler<AddDistributionPlatformToSubscriptionCommand>
    {
        private readonly ActorSystem _actorSystem;

        public AddDistributionPlatformToSubscriptionCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem;

        public Task Handle(AddDistributionPlatformToSubscriptionCommand message, CancellationToken cancellationToken)
        {
            var subscriptionActor = _actorSystem.ActorOf(SubscriptionActor.GetProps(message.SubscriptionId));
            subscriptionActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}