using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class AddSubscriptionToReleaseCommandHandler : IRequestHandler<AddSubscriptionToReleaseCommand>
    {
        private readonly ActorSystem _actorSystem;

        public AddSubscriptionToReleaseCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem ?? throw new System.ArgumentNullException(nameof(actorSystem));

        public Task Handle(AddSubscriptionToReleaseCommand message, CancellationToken cancellationToken)
        {
            var subscriptionActor = _actorSystem.ActorOf(ReleaseActor.GetProps(message.ReleaseId));
            subscriptionActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}